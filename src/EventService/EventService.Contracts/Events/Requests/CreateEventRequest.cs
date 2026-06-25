using EventService.Contracts.DTO;

namespace EventService.Contracts.Events.Requests;

public sealed record CreateEventRequest(
    string Title,
    Guid? EventTypeId,
    DateTime StartDate,
    DateTime EndDate,
    int MaxParticipants,
    Guid OrganizerId,
    string? Description,
    LocationFullDto Location);
