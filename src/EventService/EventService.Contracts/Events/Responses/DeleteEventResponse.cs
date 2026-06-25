namespace EventService.Contracts.Events.Responses;

public sealed record DeleteEventResponse(bool Success, DateTime DeletedAt);
