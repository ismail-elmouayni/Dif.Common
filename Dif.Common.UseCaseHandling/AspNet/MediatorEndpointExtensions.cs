namespace Dif.Common.UseCaseHandling.AspNet;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Dif.Common.UseCaseHandling.Handlers;
using Dif.Common.UseCaseHandling.Pipeline;
using System.Reflection;

public static class MediatorEndpointExtensions
{
    // Cache the open generic MethodInfo to avoid repeated reflection on every endpoint registration.
    private static readonly MethodInfo s_getQueryHandlerMethod =
        typeof(MediatorEndpointExtensions)
            .GetMethod(nameof(GetQueryHandler), BindingFlags.NonPublic | BindingFlags.Static)!;

    public static RouteHandlerBuilder MapPostCommand<TCommand, TResponse>(
        this IEndpointRouteBuilder endpoints,
        string pattern)
        where TCommand : IUseCase
    {
        return endpoints.MapPost(pattern, async (TCommand command, [FromServices] IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.SendAsync<TCommand, TResponse>(command, ct);
            return result.ToHttpResult();
        });
    }

    public static RouteHandlerBuilder MapPostCommand<TCommand>(
        this IEndpointRouteBuilder endpoints,
        string pattern)
        where TCommand : IUseCase
    {
        return endpoints.MapPost(pattern, async (TCommand command, [FromServices] IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.SendAsync(command, ct);
            return result.ToHttpResult();
        });
    }

    public static RouteHandlerBuilder MapPutCommand<TCommand, TResponse>(
       this IEndpointRouteBuilder endpoints,
       string pattern)
       where TCommand : IUseCase
    {
        return endpoints.MapPut(pattern, async (TCommand command, [FromServices] IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.SendAsync<TCommand, TResponse>(command, ct);
            return result.ToHttpResult();
        });
    }

    public static RouteHandlerBuilder MapPutCommand<TCommand>(
        this IEndpointRouteBuilder endpoints,
        string pattern)
        where TCommand : IUseCase
    {
        return endpoints.MapPut(pattern, async (TCommand command, [FromServices] IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.SendAsync(command, ct);
            return result.ToHttpResult();
        });
    }

    public static RouteHandlerBuilder MapPatchCommand<TCommand>(
        this IEndpointRouteBuilder endpoints,
        string pattern)
        where TCommand : IUseCase
    {
        return endpoints.MapPatch(pattern, async ([FromBody] TCommand command, [FromServices] IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.SendAsync(command, ct);
            return result.ToHttpResult();
        });
    }

    public static RouteHandlerBuilder MapPatchCommand<TCommand, TResponse>(
        this IEndpointRouteBuilder endpoints,
        string pattern)
        where TCommand : IUseCase
    {
        return endpoints.MapPatch(pattern, async ([FromBody] TCommand command, [FromServices] IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.SendAsync<TCommand, TResponse>(command, ct);
            return result.ToHttpResult();
        });
    }

    public static RouteHandlerBuilder MapGetQuery<TQuery, TResponse>(
        this IEndpointRouteBuilder endpoints,
        string pattern)
        where TQuery : IUseCase
    {
        var closedMethod = s_getQueryHandlerMethod.MakeGenericMethod(typeof(TQuery), typeof(TResponse));
        var handlerDelegate = closedMethod.CreateDelegate(
            typeof(Func<TQuery, IMediator, CancellationToken, Task<IResult>>));

        return endpoints.MapGet(pattern, handlerDelegate);
    }

    private static async Task<IResult> GetQueryHandler<TQuery, TResponse>(
        [AsParameters] TQuery query,
        [FromServices] IMediator mediator,
        CancellationToken ct)
        where TQuery : IUseCase
    {
        var result = await mediator.SendAsync<TQuery, TResponse>(query, ct);
        return result.ToHttpResult();
    }
}
