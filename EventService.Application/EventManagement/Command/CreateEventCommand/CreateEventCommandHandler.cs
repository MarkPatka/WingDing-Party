using EventService.Application.EventManagement.Common;
using EventService.Application.Persistence;
using EventService.Application.Services;
using EventService.Domain;
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
            .CheckEventNotExists(request.Title, request.OrganizerId, cancellationToken); /// NEW 

        if (eventExists)
            throw new Exception($"Event already exists");

        // create
        var newEvent = Event.Create(
            request.Title,
            request.Description!,
            request.EventType,
            request.Location!,
            request.StartDate,
            request.EndDate,
            request.MaxParticipants,
            request.OrganizerId,
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
