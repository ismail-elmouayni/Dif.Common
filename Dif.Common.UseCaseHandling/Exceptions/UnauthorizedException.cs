namespace Dif.Common.UseCaseHandling.Exceptions;

using FluentResults;

/// <summary>
/// Exception for authorization failures.
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public override IError ToError() => new Error(Message)
        .WithMetadata("ErrorType", "Unauthorized");
}
