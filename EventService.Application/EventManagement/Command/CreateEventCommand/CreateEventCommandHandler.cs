using EventService.Domain;
using MediatR;
using EventService.Application.EventManagement.Common;
using EventService.Application.Persistence;
using EventService.Application.Services;

namespace EventService.Application.EventManagement.Command.CreateEventCommand;

public class CreateEventCommandHandler
    : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    private readonly IEventRepository _repository;
    private readonly ITimeProviderService _timeProvider;

    public CreateEventCommandHandler(IEventRepository repository, ITimeProviderService timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<CreateEventResult> Handle(
        CreateEventCommand request,
        CancellationToken cancellationToken)
    {
        // check if not exists
        var existingEventFromDb = await _repository.GetByFilter(x =>
                x.Title == request.Title &&
                x.EventType == request.EventType &&
                x.StartDate == request.StartDate &&
                x.OrganizerId == request.OrganizerId);

        if (existingEventFromDb != null)
        {
            throw new Exception($"Event already exists");
        }

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
        await _repository.Add(newEvent);

        // return result
        var result = new CreateEventResult(
                    newEvent.Id.Value,
                    newEvent.CreatedAt,
                    newEvent.UpdatedAt,
                    newEvent.Status);

        return await Task.FromResult(result);
    }
}
