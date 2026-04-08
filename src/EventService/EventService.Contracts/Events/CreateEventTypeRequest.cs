namespace EventService.Contracts.Events;

public sealed record CreateEventTypeRequest(
    string Name,
    string? Description,
    string? Icon);
