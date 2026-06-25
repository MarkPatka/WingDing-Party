using EventService.Domain;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Application.Persistence.Specifications;

public class EventParticipantsByEventIdSpecification : BaseSpecification<Event>
{
    public EventParticipantsByEventIdSpecification(EventId eventId)
        : base(e => e.Id == eventId)
    {
        AddInclude(e => e.Participants);
        ApplyNoTracking();
        ApplySplitQuery();
    }
}
