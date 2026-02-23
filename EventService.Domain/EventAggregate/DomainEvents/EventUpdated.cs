using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Domain.EventAggregate.DomainEvents;

public sealed record EventUpdated(EventId EventId, DateTime UpdatedAt) : IDomainEvent
{
    public DateTime OccurredOn => UpdatedAt;
}
