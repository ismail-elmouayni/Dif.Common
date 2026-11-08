namespace Dif.Common.UseCaseHandling.Exceptions;

using Dif.Common.ResultExtensions.Errors;
using FluentResults;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// Maps to NotFoundError.
/// </summary>
public class NotFoundException : DomainException
{
    public string? ResourceType { get; }
    public string? ResourceId { get; }

    public NotFoundException(string message, string? resourceType = null, string? resourceId = null)
        : base(message)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    public override IError ToError() => new NotFoundError(Message, ResourceType, ResourceId);
}
