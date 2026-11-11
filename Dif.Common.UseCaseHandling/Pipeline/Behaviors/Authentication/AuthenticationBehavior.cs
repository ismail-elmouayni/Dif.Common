namespace Dif.Common.UseCaseHandling.Pipeline.Behaviors.Authentication;

using Dif.Common.UseCaseHandling.Exceptions;
using FluentResults;

/// <summary>
/// Opt-in authentication gate: a no-op pass-through for any request that does not implement
/// <see cref="IRequireAuthentication"/>, so pre-existing requests/handlers are unaffected. For requests that do
/// implement it, throws <see cref="UnauthorizedException"/> when <see cref="ICurrentPrincipalAccessor.IsAuthenticated"/>
/// is false; <see cref="ExceptionHandlingBehavior{TRequest, TResponse}"/> (registered by
/// <c>AddMediatorPipeline</c>, which always wraps outside any behavior added afterwards) converts it to a failed
/// <see cref="Result{TResponse}"/>.
/// </summary>
public class AuthenticationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentPrincipalAccessor _principal;

    public AuthenticationBehavior(ICurrentPrincipalAccessor principal)
    {
        _principal = principal;
    }

    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken cancellationToken)
    {
        if (request is IRequireAuthentication && !_principal.IsAuthenticated)
        {
            throw new UnauthorizedException("Authentication is required to perform this request.");
        }

        return await next();
    }
}
