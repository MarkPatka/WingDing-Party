using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Domain.EventAggregate.DomainEvents;

public sealed record ParticipantRegistered(
    EventId EventId,
    ParticipantId ParticipantId,
    DateTime RegisteredAt) : IDomainEvent
{
    public DateTime OccurredOn => RegisteredAt;
}
