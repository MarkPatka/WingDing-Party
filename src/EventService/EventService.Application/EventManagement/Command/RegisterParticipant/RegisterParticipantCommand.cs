using EventService.Application.EventManagement.Common;
using EventService.Domain.EventAggregate.ValueObjects;
using MediatR;

namespace EventService.Application.EventManagement.Command.RegisterParticipant;

public record RegisterParticipantCommand(
    EventId EventId,
    UserId UserId,
    string UserName
    ) : IRequest<RegisterParticipantResult>;
