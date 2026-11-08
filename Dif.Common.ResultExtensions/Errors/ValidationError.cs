namespace Dif.Common.ResultExtensions.Errors;

using FluentResults;

/// <summary>
/// Custom error for validation failures
/// </summary>
public class ValidationError : Error
{
    public string? PropertyName { get; }

    public ValidationError(string message, string? propertyName = null)
        : base(message)
    {
        PropertyName = propertyName;
    }

    public ValidationError(string message, string? propertyName, IError? causedBy)
        : base(message, causedBy)
    {
        PropertyName = propertyName;
    }
}
