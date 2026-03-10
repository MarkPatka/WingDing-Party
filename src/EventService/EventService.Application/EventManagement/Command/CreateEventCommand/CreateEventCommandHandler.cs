using EventService.Application.EventManagement.Common;
using EventService.Application.Persistence;
using EventService.Application.Services;
using EventService.Domain;
using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;
using MediatR;

namespace EventService.Application.EventManagement.Command.CreateEventCommand;

public class CreateEventCommandHandler
    : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    private readonly IEventService _eventService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventCommandHandler(
        IEventService eventService,
        IUnitOfWork unitOfWork)
    {
        _eventService = eventService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateEventResult> Handle(
        CreateEventCommand request,
        CancellationToken cancellationToken)
    {
        // check if not exists
        var eventExists = await _eventService
            .CheckEventNotExists(
                request.Title, 
                UserId.Create(request.OrganizerId), 
                cancellationToken);

        if (eventExists)
            throw new Exception($"Event already exists");

        // create
        var newEvent = Event.Create(
            request.Title,
            request.Description!,
            eventType: EventType.CreateNew(
                    Guid.NewGuid(),
                    "DotNext",
                    "Conference",
                    "Persisted_Icon_In_MiniO_Storage"),
            request.Location!,
            request.StartDate,
            request.EndDate,
            request.MaxParticipants,
            UserId.Create(request.OrganizerId),
            TimeProvider.System.GetUtcNow().DateTime,
            TimeProvider.System.GetUtcNow().DateTime
        );

        // add
        await _eventService.CreateEventAsync(newEvent, cancellationToken);

        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        // return result
        return new CreateEventResult(
                    newEvent.Id.Value,
                    newEvent.CreatedAt,
                    newEvent.UpdatedAt,
                    newEvent.Status.Id);
    }
}
