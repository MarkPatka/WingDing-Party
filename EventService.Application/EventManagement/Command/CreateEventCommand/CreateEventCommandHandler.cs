using EventService.Domain;
using MediatR;
using EventService.Application.EventManagement.Common;
using EventService.Application.Services;

namespace EventService.Application.EventManagement.Command.CreateEventCommand;

public class CreateEventCommandHandler
    : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    private readonly IEventService _eventService;
    private readonly ITimeProviderService _timeProvider;

    public CreateEventCommandHandler(
        IEventService eventService,
        ITimeProviderService timeProvider)
    {
        _eventService = eventService;
        _timeProvider = timeProvider;
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
            _timeProvider.UtcNow,
            _timeProvider.UtcNow
        );

        // add
        await _eventService.CreateEventAsync(newEvent, cancellationToken);

        // return result
        return new CreateEventResult(
                    newEvent.Id.Value,
                    newEvent.CreatedAt,
                    newEvent.UpdatedAt,
                    newEvent.Status.Id);
    }
}
