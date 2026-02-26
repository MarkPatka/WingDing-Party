using System.Linq.Expressions;

namespace EventService.Application.Persistence;

public interface ISpecification<TEntity>
    where TEntity : class
{
    Expression<Func<TEntity, bool>>? Criteria { get; }
    List<Expression<Func<TEntity, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    List<SearchTerm<TEntity>> SearchTerms { get; }

    Expression<Func<TEntity, object>>? OrderBy { get; }
    Expression<Func<TEntity, object>>? OrderByDescending { get; }
    Expression<Func<TEntity, object>>? GroupBy { get; }

    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }

    bool AsNoTracking { get; }
    bool AsSplitQuery { get; }
}

public record SearchTerm<TEntity>(
    Expression<Func<TEntity, string>> PropertySelector, // e => e.Title
    string Value                                        // "concert"
) where TEntity : class;