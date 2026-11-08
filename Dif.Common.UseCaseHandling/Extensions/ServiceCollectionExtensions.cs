namespace Dif.Common.UseCaseHandling.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Dif.Common.UseCaseHandling.Handlers;
using Dif.Common.UseCaseHandling.Messaging;
using Dif.Common.UseCaseHandling.Pipeline;
using Dif.Common.UseCaseHandling.Pipeline.Behaviors;
using Dif.Common.UseCaseHandling.Registry;
using Dif.Common.UseCaseHandling.Validation;
using System.Reflection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatorPipeline(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<IPipelineExecutor, PipelineExecutor>();

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PublishBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AtomicBehavior<,>));

        if (assemblies.Length > 0)
        {
            foreach (var assembly in assemblies)
            {
                services.AddHandlers(assembly);
            }
        }

        return services;
    }

    public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
    {
        var commandHandlerType = typeof(ICommandHandler<,>);
        var commandHandlerVoidType = typeof(ICommandHandler<>);
        var queryHandlerType = typeof(IQueryHandler<,>);
        var validatorType = typeof(IValidator<>);
        var commandPublisherType = typeof(IRequestPublisher<,>);
        var commandPublisherVoidType = typeof(IRequestPublisher<>);

        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToList();

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && (
                    i.GetGenericTypeDefinition() == commandHandlerType ||
                    i.GetGenericTypeDefinition() == commandHandlerVoidType))
                .ToList();

            foreach (var iface in interfaces)
                services.AddScoped(iface, type);
        }

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == queryHandlerType)
                .ToList();

            foreach (var iface in interfaces)
                services.AddScoped(iface, type);
        }

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType)
                .ToList();

            foreach (var iface in interfaces)
                services.AddScoped(iface, type);
        }

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && (
                    i.GetGenericTypeDefinition() == commandPublisherType ||
                    i.GetGenericTypeDefinition() == commandPublisherVoidType))
                .ToList();

            foreach (var iface in interfaces)
                services.AddScoped(iface, type);
        }

        services.AddScoped<IHandlerRegistry, HandlerRegistry>();

        return services;
    }

    public static IServiceCollection AddNonTransactionalMediatorPipe(
       this IServiceCollection services,
       params Assembly[] assemblies)
    {
        services.AddScoped<IMediator, Mediator>()
                .AddScoped<IPipelineExecutor, PipelineExecutor>()
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(PublishBehavior<,>));

        if (assemblies.Length > 0)
        {
            foreach (var assembly in assemblies)
            {
                _ = services.AddHandlers(assembly);
            }
        }

        return services;
    }
}
