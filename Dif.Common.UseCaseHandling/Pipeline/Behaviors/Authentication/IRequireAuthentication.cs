namespace Dif.Common.UseCaseHandling.Pipeline.Behaviors.Authentication;

/// <summary>
/// Opt-in marker for requests that must run as an authenticated principal.
/// Requests that do not implement this interface are unaffected by <see cref="AuthenticationBehavior{TRequest, TResponse}"/>.
/// </summary>
public interface IRequireAuthentication
{
}
