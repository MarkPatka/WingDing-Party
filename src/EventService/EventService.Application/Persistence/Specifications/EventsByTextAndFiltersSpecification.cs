using EventService.Domain.EventAggregate;

namespace EventService.Application.Persistence.Specifications;

public class EventsByTextAndFiltersSpecification : BaseSpecification<Event>
{
    public EventsByTextAndFiltersSpecification(
        string text,
        string? city,
        DateTime? dateFrom,
        DateTime? dateTo,
        int pageNumber,
        int pageSize) : base()
    {
        AddInclude(e => e.EventType);

        AddOrCriteriasIntoAndGroup(
            e => e.Title.ToLower().Contains(text.ToLower()),
            e => e.Description.ToLower().Contains(text.ToLower()));

        if (!string.IsNullOrWhiteSpace(city))
            AddAndCriteria(e => e.Location.City.ToLower().Contains(city.ToLower()));

        if (dateFrom.HasValue)
            AddAndCriteria(e => e.StartDate >= dateFrom.Value);

        if (dateTo.HasValue)
            AddAndCriteria(e => e.EndDate <= dateTo.Value);

        ApplyOrderBy(e => e.Title.ToLower().Contains(text.ToLower()) ? 0 : 1);
        ApplyOrderBy(e => e.StartDate);

        ApplyPaging(
            skip: (pageNumber - 1) * pageSize,
            take: pageSize);

        ApplyNoTracking();
        ApplySplitQuery();
    }
}
