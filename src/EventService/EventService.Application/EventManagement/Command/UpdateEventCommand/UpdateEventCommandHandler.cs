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
        Console.WriteLine($"🔍 Looking for EventId: {request.EventId.Value}");

        var @event = await _eventService
            .GetEventByIdAsync(request.EventId, cancellationToken);

        Console.WriteLine($"🔍 Event found: {@event != null}");

        if (@event is null)
        {
            Console.WriteLine("❌ EVENT NOT FOUND!");
            throw new EntityNotFoundException($"Event {request.EventId} not found");
        }

        @event.Update(
            request.Title,
            request.Description,
            request.Location,
            request.StartDate,
            request.EndDate,
            request.MaxParticipants);

        await _eventService.UpdateEventAsync(@event, cancellationToken);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        Console.WriteLine($"✅ Updated Title: {@event.Title}");

        return new UpdateEventResult(
            @event.Id.Value,
            @event.UpdatedAt ?? DateTime.UtcNow,
            @event.Status.Id);
    }
}
