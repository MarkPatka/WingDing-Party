using EventService.Domain;

namespace EventService.Application.Persistence.Specifications;

public class SearchEventsSpecification : BaseSpecification<Event>
{
    public SearchEventsSpecification(
        string query,
        string? eventType,
        string? city,
        DateTime? dateFrom,
        DateTime? dateTo,
        int pageNumber,
        int pageSize)
    {
        AddSearchTerm(e => e.Title, query);
        AddSearchTerm(e => e.Description, query);

        if (!string.IsNullOrWhiteSpace(eventType))
        {
            AddSearchTerm(e => e.EventType.Name, eventType);
            AddSearchTerm(e => e.EventType.Description!, eventType);
        }

        if (!string.IsNullOrWhiteSpace(city))
            AddSearchTerm(e => e.Location.City, city);

        if (dateFrom.HasValue)
            AddCriteria(e => e.StartDate >= dateFrom.Value);

        if (dateTo.HasValue)
            AddCriteria(e => e.EndDate <= dateTo.Value);

        ApplyOrderBy(e => 
            e.Title.ToLower().Contains(query.ToLower()) ? 0 : 1);
        ApplyOrderBy(e => e.StartDate);

        ApplyPaging(
            skip: (pageNumber - 1) * pageSize,
            take: pageSize);

        ApplyNoTracking();
    }
}
