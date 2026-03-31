using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Application.Persistence.Specifications;

public class EventTypeByIdSpecification : BaseSpecification<EventType>
{
    public EventTypeByIdSpecification(EventTypeId id)
        : base(et => et.Id == id)
    {
        ApplyNoTracking();
    }
}
