using EventService.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventService.Infrastructure.Persistence;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> GetQuery<TEntity>(
        IQueryable<TEntity> inputQuery,
        ISpecification<TEntity> specification)
        where TEntity : class
    {
        var query = inputQuery;

        if (specification.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        query = specification.Includes
            .Aggregate(
            seed: query,
            func: (current, include) => current.Include(include));

        query = specification.IncludeStrings
            .Aggregate(
            seed: query,
            func: (current, include) => current.Include(include));

        if (specification.OrderExpressions.Any())
        {
            var first = specification.OrderExpressions.First();
            query = first.IsDescending
                ? query.OrderByDescending(first.Expression)
                : query.OrderBy(first.Expression);

            foreach (var order in specification.OrderExpressions.Skip(1))
            {
                query = order.IsDescending
                    ? ((IOrderedQueryable<TEntity>)query).ThenByDescending(order.Expression)
                    : ((IOrderedQueryable<TEntity>)query).ThenBy(order.Expression);
            }
        }

        if (specification.GroupBy != null)
        {
            query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
        }

        if (specification.IsPagingEnabled)
        {
            query = query
                .Skip(specification.Skip)
                .Take(specification.Take);
        }

        if (specification.AsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        return query;
    }
}
