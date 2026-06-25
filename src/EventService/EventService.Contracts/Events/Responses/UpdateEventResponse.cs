namespace EventService.Contracts.Events.Responses;

public sealed record UpdateEventResponse(
    Guid EventId, DateTime UpdatedAt, int EventStatusId);
