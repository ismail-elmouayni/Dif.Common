namespace Dif.Common.UseCaseHandling.Pipeline.Behaviors;

using Dif.Common.UseCaseHandling.Exceptions;
using FluentResults;
using Microsoft.Extensions.Logging;

public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DomainException domainEx)
        {
            _logger.LogWarning(
                domainEx,
                "Domain exception occurred handling {RequestName}: {Message}",
                typeof(TRequest).Name,
                domainEx.Message);

            return Result.Fail<TResponse>(domainEx.ToError());
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception occurred handling {RequestName}",
                typeof(TRequest).Name);

            throw;
        }
    }
}
