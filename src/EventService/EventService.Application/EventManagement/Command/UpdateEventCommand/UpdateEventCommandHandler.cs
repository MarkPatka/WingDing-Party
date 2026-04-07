using EventService.Application.Common.Exceptions;
using EventService.Application.EventManagement.Common;
using EventService.Application.Persistence;
using EventService.Application.Services;
using MediatR;

namespace EventService.Application.EventManagement.Command.UpdateEventCommand;

public class UpdateEventCommandHandler
    : IRequestHandler<UpdateEventCommand, UpdateEventResult>
{
    private readonly IEventService _eventService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEventCommandHandler(
        IEventService eventService,
        IUnitOfWork unitOfWork)
    {
        _eventService = eventService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateEventResult> Handle(
        UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await _eventService
            .GetEventByIdAsync(request.EventId, cancellationToken);

        if (@event is null)
            throw new EntityNotFoundException($"Event {request.EventId} not found");

        @event.Update(
            request.Title,
            request.Description,
            request.Location,
            request.StartDate,
            request.EndDate,
            request.MaxParticipants);

        await _eventService.UpdateEventAsync(@event, cancellationToken);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return new UpdateEventResult(
            @event.Id.Value,
            @event.UpdatedAt ?? DateTime.UtcNow,
            @event.Status.Id);
    }
}
