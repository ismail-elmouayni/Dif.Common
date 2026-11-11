namespace Dif.Common.UseCaseHandling.Pipeline.Behaviors.Authentication;

/// <summary>
/// Minimal, framework-agnostic boolean gate used by <see cref="AuthenticationBehavior{TRequest, TResponse}"/>.
/// Consuming projects provide the implementation (e.g. reading the current <c>ClaimsPrincipal</c>).
/// </summary>
public interface ICurrentPrincipalAccessor
{
    bool IsAuthenticated { get; }
}
