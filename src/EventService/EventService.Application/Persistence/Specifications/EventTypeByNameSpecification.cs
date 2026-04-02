using EventService.Domain.EventAggregate.Entities;

namespace EventService.Application.Persistence.Specifications;

public class EventTypeByNameSpecification : BaseSpecification<EventType>
{
    public EventTypeByNameSpecification(string name)
    {
        AddAndCriteria(et => et.Name == name);
        ApplyNoTracking();
    }
}
