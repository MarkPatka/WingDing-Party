using EventService.Domain;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Application.Persistence.Specifications;

public class EventByIdSpec : BaseSpecification<Event>
{
    public EventByIdSpec(EventId eventId)
        : base(e => e.Id == eventId)
    {
        AddInclude(e => e.Participants);
        AddInclude(e => e.Reviews);
        ApplySplitQuery();
    }
}
