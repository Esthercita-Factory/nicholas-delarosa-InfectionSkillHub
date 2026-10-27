using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Domain.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace InfectionSkillHub.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            // The services already check duplicates; the unique index has the last word on races.
            throw new DuplicateEntityException("A record with the same unique data already exists.");
        }
    }

    public async Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => new EfTransactionScope(await _db.Database.BeginTransactionAsync(cancellationToken));

    public void DiscardChanges() => _db.ChangeTracker.Clear();

    private static bool IsUniqueViolation(DbUpdateException exception)
        => exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    private sealed class EfTransactionScope : ITransactionScope
    {
        private readonly IDbContextTransaction _transaction;

        public EfTransactionScope(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default) => _transaction.CommitAsync(cancellationToken);

        public Task RollbackAsync(CancellationToken cancellationToken = default) => _transaction.RollbackAsync(cancellationToken);

        public ValueTask DisposeAsync() => _transaction.DisposeAsync();
    }
}
