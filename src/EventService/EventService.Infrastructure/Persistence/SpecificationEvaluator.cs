using EventService.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        foreach (var searchTerm in specification.SearchTerms)
        {
            var likeExpression = CreateLikeExpression(searchTerm);
            query = query.Where(likeExpression);
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

    private static Expression<Func<TEntity, bool>> CreateLikeExpression<TEntity>(SearchTerm<TEntity> searchTerm) 
        where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var propertyAccess = Expression.Invoke(searchTerm.PropertySelector, parameter);

        // preperty.ToLower()
        var toLowerProperty = Expression.Call(propertyAccess,
            typeof(string).GetMethod("ToLower")!);

        // EF.Functions
        var efFunctions = Expression.Constant(EF.Functions);

        // "%term%"
        var pattern = $"%{searchTerm.Value}%";

        // EF.Functions.Like(toLowerProperty, "%term%")
        var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
            nameof(DbFunctionsExtensions.Like),
            [typeof(DbFunctions), typeof(string), typeof(string)])!;

        var likeCall = Expression.Call(likeMethod, efFunctions, toLowerProperty,
            Expression.Constant(pattern));

        return Expression.Lambda<Func<TEntity, bool>>(likeCall, parameter);
    }
}
