namespace EventService.Contracts.Events.Responses;

public sealed record CreateEventResponse(
    Guid EventId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int EventStatusId);