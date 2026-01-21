using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Contracts.Events;

public sealed record CreateEventRequest(
    string Title,
    EventType EventType,
    DateTime StartDate,
    DateTime EndDate,
    int MaxParticipants,
    OrganizerId OrganizerId,
    string? Description,
    Location? Location);
