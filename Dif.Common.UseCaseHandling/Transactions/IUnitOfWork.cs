namespace Dif.Common.UseCaseHandling.Transactions;

// TODO : see todo of SqlStreamStoreUnitOfWork
/// <summary>
/// Abstraction for atomic operation scope management.
/// Decoupled from any specific persistence technology.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Begins an atomic scope. Returns a handle that must be committed or will roll back on disposal.
    /// </summary>
    Task<IAtomicTransaction> BeginAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists any pending in-memory changes to the underlying store.
    /// For event-sourced stores this is typically a no-op; the append IS the save.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// True when an atomic scope is currently active.
    /// </summary>
    bool IsActive { get; }
}
