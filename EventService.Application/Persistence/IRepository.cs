using System.Linq.Expressions;

namespace EventService.Application.Persistence;

public interface IRepository<TEntity>
{
    public Task Add(TEntity entity);
    public Task<TEntity> Get(Expression<Func<TEntity, bool>> expression);
    public Task<IEnumerable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> expression);
    public Task Update(TEntity entity);
    public Task Delete(TEntity entity);
}
