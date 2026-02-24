using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Domain.EventAggregate.DomainEvents;

public sealed record EventCreated(EventId EventId, DateTime CreatedAt) : IDomainEvent
{
    public DateTime OccurredOn => CreatedAt;
}