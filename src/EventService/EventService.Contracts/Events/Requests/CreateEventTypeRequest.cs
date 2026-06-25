namespace EventService.Contracts.Events.Requests;

public sealed record CreateEventTypeRequest(
    string Name,
    string? Description,
    string? Icon);
