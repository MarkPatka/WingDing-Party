using EventService.Domain.EventAggregate;
using EventService.Domain.EventAggregate.Enumerations;

namespace EventService.Application.Persistence.Specifications;

public class ActiveEventsSpecification : BaseSpecification<Event>
{
    public ActiveEventsSpecification(DateTime fromDate)
    {
        AddAndCriteria(e => e.Status == EventStatus.Active);
        AddAndCriteria(e => e.StartDate >= fromDate);
        ApplyOrderBy(e => e.StartDate);
        ApplyNoTracking();
    }
}
