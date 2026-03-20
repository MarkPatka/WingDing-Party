using System.Data;
using System.Text.Json;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using UserService.Application.IntegrationEvents;
using UserService.Application.Persistence;
using UserService.Domain.Common.Abstract;
using UserService.Infrastructure.Persistence.Outbox;

namespace UserService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly UserServiceDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IMapper _mapper;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    public UnitOfWork(
        UserServiceDbContext context,
        IMapper mapper,
        ILogger<UnitOfWork> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(mapper);

        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await DispatchDomainEventsAsync(cancellationToken);
            var result = await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Saved {Count} entities to database", result);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency exception occurred while saving changes");
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update exception occurred while saving changes");
            throw;
        }
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
        
        var integrationEvents = domainEvents
            .Select(de => _mapper.Map<IDomainEvent, IIntegrationEvent>(de))
            .ToList();
        
        foreach (var ie in integrationEvents)
        {
            var payload = JsonSerializer.Serialize(ie);
            var message = new OutboxMessage(ie, payload, ie.GetType().Name!);
            await _context.OutboxMessages.AddAsync(message, cancellationToken);
        }
        domainEntities.ForEach(e => e.ClearDomainEvents());


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