using EventService.Application.Common.Exceptions;
using EventService.Application.EventManagement.Common;
using EventService.Application.Persistence;
using EventService.Application.Services;
using MediatR;

namespace EventService.Application.EventManagement.Command.RegisterParticipant;

public class RegisterParticipantCommandHandler
    : IRequestHandler<RegisterParticipantCommand, RegisterParticipantResult>
{
    private readonly IEventService _eventService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterParticipantCommandHandler(
        IEventService eventService,
        IUnitOfWork unitOfWork)
    {
        _eventService = eventService;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterParticipantResult> Handle(
        RegisterParticipantCommand request, CancellationToken cancellationToken)
    {
        var @event = await _eventService
            .GetEventByIdAsync(request.EventId, cancellationToken);

        if (@event is null)
            throw new EntityNotFoundException($"Event {request.EventId} not found");

        var participant = @event.RegisterParticipant(request.UserId, request.UserName);

        //await _eventService.
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return new RegisterParticipantResult(participant.Id.Value, participant.RegisteredAt);
    }
}
