using EventService.Domain.EventAggregate.Entities;

namespace EventService.Application.Persistence.Specifications;

public class DefaultEventTypeSpecification : BaseSpecification<EventType>
{
    public DefaultEventTypeSpecification()
        : base(et => et.IsDefault)
    {
        ApplyNoTracking();
    }
}
