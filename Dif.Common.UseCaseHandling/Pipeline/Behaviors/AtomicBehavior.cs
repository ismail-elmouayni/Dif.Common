namespace Dif.Common.UseCaseHandling.Pipeline.Behaviors;

using Dif.Common.UseCaseHandling.Transactions;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class AtomicBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AtomicBehavior<TRequest, TResponse>> _logger;

    public AtomicBehavior(
        IServiceProvider serviceProvider,
        ILogger<AtomicBehavior<TRequest, TResponse>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken cancellationToken)
    {
        if (request is not ITransactionalRequest)
            return await next();

        var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

        if (unitOfWork.IsActive)
        {
            _logger.LogDebug("Already in atomic scope, skipping for {RequestName}", typeof(TRequest).Name);
            return await next();
        }

        _logger.LogDebug("Beginning atomic scope for {RequestName}", typeof(TRequest).Name);

        await using var transaction = await unitOfWork.BeginAsync(cancellationToken);

        try
        {
            var result = await next();

            if (result.IsSuccess)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                _logger.LogDebug("Atomic scope committed for {RequestName}", typeof(TRequest).Name);
            }
            else
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogWarning("Atomic scope rolled back for {RequestName} (failed result)", typeof(TRequest).Name);
            }

            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Atomic scope rolled back for {RequestName} (exception)", typeof(TRequest).Name);
            throw;
        }
    }
}
