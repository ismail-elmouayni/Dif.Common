namespace Dif.Common.UseCaseHandling.Handlers;

using FluentResults;

/// <summary>
/// Handler interface for queries (read-only operations).
/// Queries represent read operations in CQRS pattern.
/// </summary>
public interface IQueryHandler<TQuery, TResponse>
{
    /// <summary>
    /// Handles the query execution.
    /// </summary>
    /// <param name="query">The query to execute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the response or errors</returns>
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
