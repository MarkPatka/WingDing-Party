using EventService.Domain.Common.Abstract;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Domain.EventAggregate.DomainEvents;

public sealed record EventDeleted(EventId EventId, DateTime DeletedAt) : IDomainEvent
{
    public DateTime OccurredOn => DeletedAt;
}
