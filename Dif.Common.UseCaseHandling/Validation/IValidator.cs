namespace Dif.Common.UseCaseHandling.Validation;

using FluentResults;

public interface IValidator<TRequest>
{
    /// <summary>
    /// Validates the request and returns a Result indicating success or validation errors
    /// </summary>
    Task<Result> ValidateAsync(TRequest request, CancellationToken cancellationToken);
}
