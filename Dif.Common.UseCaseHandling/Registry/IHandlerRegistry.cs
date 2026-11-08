namespace Dif.Common.UseCaseHandling.Registry;

using Dif.Common.UseCaseHandling.Handlers;

public interface IHandlerRegistry
{
    ICommandHandler<TCommand, TResponse> GetCommandHandler<TCommand, TResponse>();
    IQueryHandler<TQuery, TResponse> GetQueryHandler<TQuery, TResponse>();
    IPostProcessor<TCommand> GetPostProcessor<TCommand>(string commandType) where TCommand : ICommand;
}
