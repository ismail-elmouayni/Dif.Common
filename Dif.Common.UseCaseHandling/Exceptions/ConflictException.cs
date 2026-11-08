using Dif.Common.ResultExtensions.Errors;
using FluentResults;

namespace Dif.Common.UseCaseHandling.Exceptions;

/// <summary>
/// Exception thrown when a business rule conflict occurs.
/// Maps to ConflictError.
/// </summary>
public class ConflictException : DomainException
{
    public string? ResourceId { get; }

    public ConflictException(string message, string? resourceId = null)
        : base(message)
    {
        ResourceId = resourceId;
    }

    public override IError ToError() => new ConflictError(Message, ResourceId);
}
