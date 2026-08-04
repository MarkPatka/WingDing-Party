using EventService.Domain.EventAggregate;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Application.Persistence.Specifications;

public class EventsByParticipantSpec : BaseSpecification<Event>
{
    public EventsByParticipantSpec(UserId userId)
    {
        AddAndCriteria(e => e.Participants.Any(p => p.UserId == userId));
        AddInclude(e => e.Participants);
        ApplySplitQuery();
    }
}
