namespace EventService.Contracts.Events;

public sealed record DeleteEventResponse(bool Success, DateTime DeletedAt);
