using EventService.Domain;
using EventService.Domain.EventAggregate.Enumerations;

namespace EventService.Application.Persistence.Specifications;

public class TopRatedEventsByStartDateWithLimitSpecification : BaseSpecification<Event>
{
    public TopRatedEventsByStartDateWithLimitSpecification(DateTime startDate, int limit)
    {
        AddAndCriteria(e => e.StartDate >= startDate);
        AddAndCriteria(e => e.Status == EventStatus.Active);

        ApplyOrderByDescending(e => e.AverageRating ?? 0m);

        ApplyPaging(skip: 0, take: limit);

        ApplyNoTracking();
    }
}
