using EventService.Application.Common.Exceptions;
using EventService.Application.EventManagement.Common;
using EventService.Application.Persistence;
using EventService.Application.Services;
using MediatR;

namespace EventService.Application.EventManagement.Command.DeleteEventCommand;

public class DeleteEventCommandHandler
    : IRequestHandler<DeleteEventCommand, DeleteEventResult>
{
    private readonly IEventService _eventService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEventCommandHandler(
        IEventService eventService,
        IUnitOfWork unitOfWork)
    {
        _eventService = eventService;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteEventResult> Handle(
        DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await _eventService
            .GetEventByIdAsync(request.EventId, cancellationToken);

        if (@event is null)
            throw new EntityNotFoundException($"Event {request.EventId} not found");

        @event.MarkAsDeleted();

        await _eventService.UpdateEventAsync(@event, cancellationToken);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return new DeleteEventResult(true, @event.UpdatedAt!.Value);
    }
}
