using EventService.Application.EventManagement.Common;
using EventService.Domain.EventAggregate.ValueObjects;
using MediatR;

namespace EventService.Application.EventManagement.Command.UpdateEventCommand;

public record UpdateEventCommand(
    EventId EventId,
    string? Title,
    string? Description,
    Location? Location,
    DateTime? StartDate,
    DateTime? EndDate,
    int? MaxParticipants
    ) 
    : IRequest<UpdateEventResult>;
