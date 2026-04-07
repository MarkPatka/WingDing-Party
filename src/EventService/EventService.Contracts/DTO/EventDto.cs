namespace EventService.Contracts.DTO;

public sealed record EventDto(
    string Id,
    string Title,
    string Description,
    EventTypeDto EventType,
    string Status,
    LocationDto Location,
    DateTime StartDate,
    DateTime EndDate,
    int MaxParticipants);
