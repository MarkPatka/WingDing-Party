using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Domain.EventAggregate.DomainEvents;

public sealed record EventPublished(EventId EventId, DateTime OccurredAt) : IDomainEvent
{
    public DateTime OccurredOn => OccurredAt;
}
