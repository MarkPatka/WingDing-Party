using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;
using MediatR;
using EventService.Application.EventManagement.Common;

namespace EventService.Application.EventManagement.Command.CreateEventCommand;

public record CreateEventCommand(
    string Title,
    Guid EventType,
    DateTime StartDate,
    DateTime EndDate,
    int MaxParticipants,
    Guid OrganizerId,
    string? Description = null,
    Location? Location = null
    ) : IRequest<CreateEventResult>;
