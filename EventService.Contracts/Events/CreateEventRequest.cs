using EventService.Contracts.DTO;
using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Contracts.Events;

public sealed record CreateEventRequest(
    string Title,
    Guid EventTypeId,
    string EventTypeName,
    string? EventTypeIcon,
    string? EventTypeDescription,
    DateTime StartDate,
    DateTime EndDate,
    int MaxParticipants,
    Guid OrganizerId,
    string? Description,
    LocationFullDto Location);
