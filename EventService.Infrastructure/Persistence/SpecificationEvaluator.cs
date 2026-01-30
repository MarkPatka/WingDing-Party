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

        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
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
