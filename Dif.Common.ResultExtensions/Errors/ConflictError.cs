namespace Dif.Common.ResultExtensions.Errors;

using FluentResults;

/// <summary>
/// Custom error for conflict/duplicate resource failures
/// </summary>
public class ConflictError : Error
{
    public string? ResourceId { get; }

    public ConflictError(string message, string? resourceId = null)
        : base(message)
    {
        ResourceId = resourceId;
    }

    public ConflictError(string message, string? resourceId = null, IError? causedBy = null)
        : base(message, causedBy)
    {
        ResourceId = resourceId;
    }
}
