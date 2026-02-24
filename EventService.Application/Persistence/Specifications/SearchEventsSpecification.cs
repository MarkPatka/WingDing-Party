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
        if (!string.IsNullOrWhiteSpace(query))
            AddCriteria(e => 
                e.Title.ToLower().Contains(query.ToLower()) ||
                e.Description.ToLower().Contains(query.ToLower()));

        if (!string.IsNullOrWhiteSpace(eventType))
            AddCriteria(e =>
                e.EventType.Name.ToLower().Contains(eventType.ToLower()) ||
                e.EventType.Description!.ToLower().Contains(eventType.ToLower()));

        if (!string.IsNullOrWhiteSpace(city))
            AddCriteria(e => e.Location.City.ToLower().Contains(city.ToLower()));

        if (dateFrom.HasValue)
            AddCriteria(e => e.StartDate == dateFrom.Value);

        if (dateTo.HasValue)
            AddCriteria(e => e.EndDate == dateTo.Value);

        ApplyPaging(
            skip: (pageNumber - 1) * pageSize,
            take: pageSize);

        ApplyOrderByDescending(e => e.StartDate);

        ApplyNoTracking();
    }
}
