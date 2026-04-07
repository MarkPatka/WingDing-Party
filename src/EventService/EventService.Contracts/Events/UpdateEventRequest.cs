using EventService.Contracts.DTO;

namespace EventService.Contracts.Events;

public sealed record UpdateEventRequest(
    Guid EventId,
    string? Title,
    string? Description,
    LocationFullDto? Location,
    DateTime? StartDate,
    DateTime? EndDate,
    int? MaxParticipants);
