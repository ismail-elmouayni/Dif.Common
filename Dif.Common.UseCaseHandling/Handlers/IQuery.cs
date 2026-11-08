namespace Dif.Common.UseCaseHandling.Handlers;

public interface IQuery<TResponse> : IUseCase
{
    bool ShouldPublish => false;
}