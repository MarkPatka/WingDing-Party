using EventService.Domain.Common.Abstract;
using Microsoft.EntityFrameworkCore;
using EventService.Application.Persistence;
using System.Linq.Expressions;

namespace EventService.Infrastructure.Persistence;

/// <summary>
/// EFCore-specific generic repository
/// </summary>
public class GenericRepository<TEntity, TId>
    : IRepository<TEntity, TId>
    where TEntity : class
    where TId : IEntityId
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public GenericRepository(EventServiceDbContext dbContext)
    {
        Context = dbContext;
        DbSet = Context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default)
    {
        var keyValues = new object[] { id.Value };
        return await DbSet.FindAsync(keyValues, cancellationToken);
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        TId id,
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        // 1. Apply specification (includes, filters, etc.) to base query
        var query = ApplySpecification(specification);

        // 2.Get EF Core metadata to find the primary key property name
        var keyProperty = Context.Model
            .FindEntityType(typeof(TEntity))?
            .FindPrimaryKey()?.Properties[0];

        // 3. Safety check - ensure entity has a primary key
        if (keyProperty == null)
            throw new InvalidOperationException($"Entity {typeof(TEntity).Name} has no primary key defined");

        // 4. Build expression tree dynamically: e => e.Id == id.Value
        var parameter = Expression.Parameter(typeof(TEntity), "e");      // Creates: e
        var property = Expression.Property(parameter, keyProperty.Name);// Creates: e.Id
        var idValue = Expression.Constant(id.Value);                   // Creates: constant value
        var equals = Expression.Equal(property, idValue);             // Creates: e.Id == value

        // 5. Wrap in lambda: e => e.Id == value
        var lambda = Expression.Lambda<Func<TEntity, bool>>(equals, parameter);

        // 6. Execute query with the dynamic filter
        return await query.FirstOrDefaultAsync(lambda, cancellationToken);
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .AnyAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public virtual Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        Context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual Task UpdateRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        DbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    protected IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
    {
        return SpecificationEvaluator.GetQuery(DbSet.AsQueryable(), specification);
    }
}