using EventService.Application.EventManagement.Common;
using EventService.Domain.EventAggregate.ValueObjects;
using MediatR;

namespace EventService.Application.EventManagement.Command.DeleteEventCommand;

public record DeleteEventCommand(EventId EventId) : IRequest<DeleteEventResult>;
