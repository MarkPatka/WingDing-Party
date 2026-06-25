using System.Data;

namespace EventService.Application.Persistence;

public interface IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, int maxRetries = 3);
    public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken =  default);
    public Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
