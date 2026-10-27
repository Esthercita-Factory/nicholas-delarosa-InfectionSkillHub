namespace InfectionSkillHub.Application.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Forgets pending changes after a failed operation, so the next one starts clean (bulk import).</summary>
    void DiscardChanges();
}

public interface ITransactionScope : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}
