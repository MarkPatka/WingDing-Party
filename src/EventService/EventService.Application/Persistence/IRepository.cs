using EventService.Domain.Common.Abstract;

namespace EventService.Application.Persistence;

public interface IRepository<TEntity, in TId> : IReadRepository<TEntity, TId>
    where TId : notnull, IEntityId
    where TEntity : class
{
    public Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    public Task<IEnumerable<TEntity>> AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    public Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    public Task UpdateRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    public Task DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    public Task DeleteRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);
}
