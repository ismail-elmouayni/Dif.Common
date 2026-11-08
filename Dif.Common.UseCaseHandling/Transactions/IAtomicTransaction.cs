namespace Dif.Common.UseCaseHandling.Transactions;

// TODO : see todo of SqlStreamStoreUnitOfWork
/// <summary>
/// Represents an in-progress atomic operation.
/// DisposeAsync rolls back if CommitAsync was not called.
/// The concrete implementation decides the mechanism
/// (EF Core IDbContextTransaction, SqlStreamStore optimistic version check, etc.).
/// </summary>
public interface IAtomicTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
