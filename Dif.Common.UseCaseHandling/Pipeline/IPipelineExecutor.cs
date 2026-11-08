namespace Dif.Common.UseCaseHandling.Pipeline;

using FluentResults;

internal interface IPipelineExecutor
{
    Task<Result<TResponse>> ExecuteAsync<TRequest, TResponse>(
        TRequest request,
        Func<TRequest, CancellationToken, Task<Result<TResponse>>> handlerDelegate,
        CancellationToken cancellationToken)
        where TRequest : notnull;
}
