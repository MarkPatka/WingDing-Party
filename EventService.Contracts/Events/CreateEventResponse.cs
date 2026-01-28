using EventService.Domain.EventAggregate.Enumerations;

namespace EventService.Contracts.Events;

public sealed record CreateEventResponse(
    Guid EventId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int EventStatusId);