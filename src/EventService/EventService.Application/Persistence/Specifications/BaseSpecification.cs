using EventService.Application.Common.Extensions;
using System.Linq.Expressions;

namespace EventService.Application.Persistence.Specifications;

public abstract class BaseSpecification<TEntity> : ISpecification<TEntity>
    where TEntity : class
{
    public Expression<Func<TEntity, bool>>? Criteria { get; private set; }
    public List<Expression<Func<TEntity, object>>> Includes { get; } = [];
    public List<string> IncludeStrings { get; } = [];

    public Expression<Func<TEntity, object>>? OrderBy { get; private set; }
    public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }
    public Expression<Func<TEntity, object>>? GroupBy { get; private set; }

    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public bool AsNoTracking { get; private set; }
    public bool AsSplitQuery { get; private set; }

    protected BaseSpecification() { }
    protected BaseSpecification(Expression<Func<TEntity, bool>> criteria) => Criteria = criteria;

    protected virtual void AddAndCriteria(Expression<Func<TEntity, bool>> criteria) =>
        Criteria = Criteria == null ? criteria : Criteria.And(criteria);

    protected virtual void AddOrCriteria(Expression<Func<TEntity, bool>> criteria) =>
        Criteria = Criteria == null ? criteria : Criteria.Or(criteria);

    // Tell EF Core to eager load related entities (prevent N+1 queries)
    // One query fetches instead of separate queries (avoids N+1 problem) 
    // USING .AddInclude(e => e.Participants)
    protected virtual void AddInclude(Expression<Func<TEntity, object>> includeExpression) =>
        Includes.Add(includeExpression);

    // Tell EF Core to eager load related entities(prevent N+1 queries)
    // One query fetches instead of separate queries (avoids N+1 problem)
    // AddInclude("Participants.Reviews")
    protected virtual void AddInclude(string includeString) =>
        IncludeStrings.Add(includeString);

    protected virtual void ApplyOrderBy(Expression<Func<TEntity, object>> orderByExpression) =>
        OrderBy = orderByExpression;

    protected virtual void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression) =>
        OrderByDescending = orderByDescExpression;

    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected virtual void ApplyGroupBy(Expression<Func<TEntity, object>> groupByExpression) =>
        GroupBy = groupByExpression;

    protected virtual void ApplyNoTracking() => AsNoTracking = true;
    protected virtual void ApplySplitQuery() => AsSplitQuery = true;
}
