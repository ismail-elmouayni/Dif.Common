using FluentResults;
namespace Dif.Common.UseCaseHandling.Exceptions;

public sealed class InvalidConfigurationException : DomainException
{
    public InvalidConfigurationException(string message)
        : base(message) { }

    public InvalidConfigurationException(string message, Exception inner)
        : base(message, inner) { }

    public override IError ToError() => new Error(Message)
        .WithMetadata("ErrorType", "InvalidConfiguration");
}