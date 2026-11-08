namespace Dif.Common.UseCaseHandling.Pipeline;

using FluentResults;

using Dif.Common.UseCaseHandling.Handlers;

public interface IMediator
{
    Task<Result<TResponse>> SendAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IUseCase;

    Task<Result> SendAsync<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IUseCase;
}
