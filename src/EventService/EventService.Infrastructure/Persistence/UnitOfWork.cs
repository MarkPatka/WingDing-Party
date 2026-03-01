using EventService.Application.Persistence;
using EventService.Domain.Common.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Data;

namespace EventService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly EventServiceDbContext _context;
    private readonly IPublisher _publisher;
    private readonly ILogger<UnitOfWork> _logger;

    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    public UnitOfWork(
        EventServiceDbContext context,
        IPublisher publisher,
        ILogger<UnitOfWork> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(publisher);
        ArgumentNullException.ThrowIfNull(logger);

        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public bool HasActiveTransaction => _currentTransaction != null;


    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default, int maxRetries = 3)
    {
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                // transaction
                // Save changes to database
                var result = await _context.SaveChangesAsync(cancellationToken);

                // table for events
                // -> transactional outbox message

                // -> commit transaction

                // Dispatch domain events before saving for transactional consistency
                await DispatchDomainEventsAsync(cancellationToken);

                _logger.LogDebug("Saved {Count} entities to database", result);

                return result;
            }
            catch (DbUpdateConcurrencyException ex) when (retryCount < maxRetries - 1)
            {
                retryCount++;

                _logger.LogWarning(
                    "Concurrency retry {Retry}/{MaxRetries}", retryCount, maxRetries);

                foreach (var entry in ex.Entries)
                    await entry.ReloadAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update exception occurred while saving changes");
                throw;
            }
        }

        throw new InvalidOperationException(
            $"Failed to save changes after {maxRetries} retries due to concurrency conflicts");

    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save entities");
            return false;
        }
    }


    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            _logger.LogWarning("Transaction already started, ignoring new transaction request");
            return;
        }

        _currentTransaction = await _context.Database
            .BeginTransactionAsync(isolationLevel, cancellationToken);

        _logger.LogInformation("Transaction {TransactionId} started", _currentTransaction.TransactionId);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No active transaction to commit");
        }

        try
        {
            // Save all pending changes
            await SaveChangesAsync(cancellationToken);

            // Commit the transaction
            await _currentTransaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Transaction {TransactionId} committed successfully",
                _currentTransaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing transaction {TransactionId}",
                _currentTransaction.TransactionId);

            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _logger.LogWarning("No active transaction to rollback");
            return;
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);

            _logger.LogWarning("Transaction {TransactionId} rolled back",
                _currentTransaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back transaction {TransactionId}",
                _currentTransaction.TransactionId);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        // Get all entities with domain events from the change tracker
        var domainEntities = _context.ChangeTracker
            .Entries<IEventSourceable>()
            .Where(x => x.Entity.DomainEvents.Count != 0)
            .Select(x => x.Entity)
            .ToList();

        if (domainEntities.Count == 0)
        {
            _logger.LogDebug("No domain events to dispatch");
            return;
        }

        // Collect all domain events
        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        _logger.LogInformation("Dispatching {Count} domain events", domainEvents.Count);

        // Clear domain events from entities BEFORE publishing
        // This prevents infinite loops if handlers modify entities
        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        // Publish domain events via MediatR
        foreach (var domainEvent in domainEvents)
        {
            _logger.LogDebug("Publishing domain event: {EventType}", domainEvent.GetType().Name);

            await _publisher.Publish(domainEvent, cancellationToken);
        }

        _logger.LogInformation("Successfully dispatched {Count} domain events", domainEvents.Count);
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_currentTransaction != null)
                {
                    _logger.LogWarning("Disposing UnitOfWork with active transaction.");
                    _currentTransaction.Rollback();
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }

                // !DO NOT dispose DbContext here if using DI!
                // The DI container manages the lifetime of DbContext
            }

            _disposed = true;
        }
    }
}

