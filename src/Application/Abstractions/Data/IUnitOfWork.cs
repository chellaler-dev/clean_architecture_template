using System.Data;

namespace Application.Abstractions.Data;

/// <summary>
/// Instead of scattering Commit/SaveChanges calls across services, 
/// IUnitOfWork centralizes transaction management (commit/rollback).
/// Supports consistency and atomicity across multiple repositories.
/// </summary>

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbTransaction> BeginTransactionAsync();
}
