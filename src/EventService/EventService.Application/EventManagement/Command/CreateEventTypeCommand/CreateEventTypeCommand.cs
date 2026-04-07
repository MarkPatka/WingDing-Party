using EventService.Application.EventManagement.Common;
using MediatR;

namespace EventService.Application.EventManagement.Command.CreateEventTypeCommand;

public record CreateEventTypeCommand(
    string Name, 
    string? Description,
    string? Icon) : IRequest<CreateEventTypeResult>;
