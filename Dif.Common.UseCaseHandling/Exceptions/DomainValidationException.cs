namespace Dif.Common.UseCaseHandling.Exceptions;

using FluentResults;
using Dif.Common.ResultExtensions.Errors;

/// <summary>
/// Exception thrown when domain validation fails.
/// Maps to ValidationError.
/// </summary>
public class DomainValidationException : DomainException
{
    public string? PropertyName { get; }

    public DomainValidationException(string message, string? propertyName = null)
        : base(message)
    {
        PropertyName = propertyName;
    }

    public override IError ToError() => new ValidationError(Message, PropertyName);
}