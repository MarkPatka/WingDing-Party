namespace EventService.Contracts.Events;

public sealed record UpdateEventResponse(
    Guid EventId, DateTime UpdatedAt, int EventStatusId);
