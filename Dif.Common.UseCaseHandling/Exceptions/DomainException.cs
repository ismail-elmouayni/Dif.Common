namespace Dif.Common.UseCaseHandling.Exceptions;

using FluentResults;

/// <summary>
/// Base exception for all domain-related exceptions.
/// These are "managed exceptions" that get mapped to Result types.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Convert the exception to a FluentResults error.
    /// </summary>
    public abstract IError ToError();
}
