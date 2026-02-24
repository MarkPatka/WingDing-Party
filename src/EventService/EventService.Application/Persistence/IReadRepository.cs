namespace EventService.Application.Persistence;

// Contravariance
// "in" - type parameter is only used as INPUT (method parameters), never as output (return types)
// Allows to use a [MORE derived](EventId) type where a [LESS derived](IEntityId) type is expected
// WITH 'in' keyword - this works: IReadRepository<Event, IEntityId> repo = GetRepository<Event, EventId>();
// WITHOUT 'in' keyword - this fails: Compiler error: Cannot convert IReadRepository<Event, EventId> to IReadRepository<Event, IEntityId>
public interface IReadRepository<TEntity, in TId>
    where TEntity : class
{
    public Task<TEntity?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default);

    public Task<TEntity?> GetByIdAsync(
        TId id,
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    public Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyList<TEntity>> ListAsync(
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyList<TEntity>> ListAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    public Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    public Task<int> CountAsync(
        CancellationToken cancellationToken = default);

    public Task<bool> AnyAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    public Task<bool> AnyAsync(
        CancellationToken cancellationToken = default);
}
