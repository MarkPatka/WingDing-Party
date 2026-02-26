using EventService.Domain;
using EventService.Domain.EventAggregate.Enumerations;

namespace EventService.Application.Persistence.Specifications;

public class RecommendedEventsSpecification : BaseSpecification<Event>
{
    public RecommendedEventsSpecification(DateTime fromDate, int limit)
    {
        AddCriteria(e => e.StartDate >= fromDate);
        AddCriteria(e => e.Status == EventStatus.Active);

        ApplyOrderByDescending(e => e.AverageRating ?? 0m);

        ApplyPaging(skip: 0, take: limit);

        ApplyNoTracking();
    }
}
