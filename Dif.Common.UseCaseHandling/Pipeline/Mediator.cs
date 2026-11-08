namespace Dif.Common.UseCaseHandling.Pipeline;

using FluentResults;

using Microsoft.Extensions.DependencyInjection;

using Dif.Common.UseCaseHandling.Handlers;

internal class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IPipelineExecutor _pipelineExecutor;

    public Mediator(IServiceProvider serviceProvider, IPipelineExecutor pipelineExecutor)
    {
        _serviceProvider = serviceProvider;
        _pipelineExecutor = pipelineExecutor;
    }

    public async Task<Result<TResponse>> SendAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IUseCase
    {
        if ( request == null )
            throw new ArgumentNullException(nameof(request));

        var handler = _serviceProvider.GetService<ICommandHandler<TRequest, TResponse>>()
            ?? _serviceProvider.GetService<IQueryHandler<TRequest, TResponse>>() as dynamic;

        if ( handler == null )
            throw new InvalidOperationException(
                $"No handler registered for {typeof(TRequest).Name}");

        return await _pipelineExecutor.ExecuteAsync<TRequest, TResponse>(
            request,
            (req, ct) => handler.HandleAsync(req, ct),
            cancellationToken);
    }

    public async Task<Result> SendAsync<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IUseCase
    {
        var voidHandler = _serviceProvider.GetService<ICommandHandler<TRequest>>();
        if ( voidHandler != null )
        {
            var lifted = await _pipelineExecutor.ExecuteAsync<TRequest, Unit>(
                request,
                async (req, ct) =>
                {
                    var r = await voidHandler.HandleAsync(req, ct);
                    return r.IsSuccess ? Result.Ok(Unit.Value) : Result.Fail<Unit>(r.Errors);
                },
                cancellationToken);
            return lifted.IsSuccess ? Result.Ok() : Result.Fail(lifted.Errors);
        }

        var result = await SendAsync<TRequest, Unit>(request, cancellationToken);
        return result.IsSuccess ? Result.Ok() : Result.Fail(result.Errors);
    }
}
