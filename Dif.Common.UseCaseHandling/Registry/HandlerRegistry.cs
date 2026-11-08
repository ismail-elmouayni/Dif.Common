namespace Dif.Common.UseCaseHandling.Registry;

using Dif.Common.UseCaseHandling.Handlers;
using Microsoft.Extensions.DependencyInjection;

public class HandlerRegistry : IHandlerRegistry
{
    private readonly IServiceProvider _serviceProvider;

    public HandlerRegistry(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICommandHandler<TCommand, TResponse> GetCommandHandler<TCommand, TResponse>()
        => _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();

    public IQueryHandler<TQuery, TResponse> GetQueryHandler<TQuery, TResponse>()
        => _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();

    public IPostProcessor<TCommand> GetPostProcessor<TCommand>(string commandType) where TCommand : ICommand
        => _serviceProvider.GetRequiredService<IPostProcessor<TCommand>>();
}
