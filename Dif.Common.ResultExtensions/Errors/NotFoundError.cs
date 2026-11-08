namespace Dif.Common.ResultExtensions.Errors;

using FluentResults;

/// <summary>
/// Custom error for resource not found failures
/// </summary>
public class NotFoundError : Error
{
    public string? ResourceId { get; }
    public string? ResourceType { get; }

    public NotFoundError(string message, string? resourceType = null, string? resourceId = null)
        : base(message)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    public NotFoundError(string message, string? resourceType = null, string? resourceId = null, IError? causedBy = null)
        : base(message, causedBy)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}
