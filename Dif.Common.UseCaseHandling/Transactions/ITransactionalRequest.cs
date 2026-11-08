namespace Dif.Common.UseCaseHandling.Transactions;

// TODO : see todo of SqlStreamStoreUnitOfWork
/// <summary>
/// Marker interface. Apply to commands whose side effects must be atomic.
/// The pipeline will call IUnitOfWork.BeginAsync / CommitAsync / RollbackAsync.
/// The concrete IUnitOfWork implementation decides the mechanism
/// (EF Core transaction, event store optimistic concurrency check, etc.).
/// </summary>
public interface ITransactionalRequest { }
