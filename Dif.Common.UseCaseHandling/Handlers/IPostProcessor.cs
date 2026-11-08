namespace Dif.Common.UseCaseHandling.Handlers;

public interface IPostProcessor<TCommand> where TCommand : ICommand
{
    Task ProcessAsync(TCommand command, CancellationToken cancellationToken);
}
