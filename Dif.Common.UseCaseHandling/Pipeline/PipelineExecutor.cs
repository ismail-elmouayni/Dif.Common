namespace Dif.Common.UseCaseHandling.Pipeline;

using FluentResults;
using Microsoft.Extensions.DependencyInjection;

internal class PipelineExecutor : IPipelineExecutor
{
    private readonly IServiceProvider _serviceProvider;

    public PipelineExecutor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<TResponse>> ExecuteAsync<TRequest, TResponse>(
        TRequest request,
        Func<TRequest, CancellationToken, Task<Result<TResponse>>> handlerDelegate,
        CancellationToken cancellationToken)
        where TRequest : notnull
    {
        var behaviors = _serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .ToList();

        RequestHandlerDelegate<Result<TResponse>> pipeline = async () =>
        {
            return await handlerDelegate(request, cancellationToken);
        };

        foreach (var behavior in behaviors)
        {
            var currentPipeline = pipeline;
            pipeline = () => behavior.HandleAsync(request, currentPipeline, cancellationToken);
        }

        return await pipeline();
    }
}
