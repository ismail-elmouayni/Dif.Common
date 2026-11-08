namespace Dif.Common.UseCaseHandling.Messaging;

using FluentResults;
using Dif.Common.UseCaseHandling.Handlers;

public interface IRequestPublisher<TRequest> where TRequest : IUseCase
{
    Task<Result> PublishAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IRequestPublisher<TRequest, TResponse> where TRequest : IUseCase
{
    Task<Result> PublishAsync(TRequest request, TResponse response, CancellationToken cancellationToken);
}