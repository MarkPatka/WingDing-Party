using EventService.Application.Common.Errors;
using EventService.Application.Common.Exceptions;
using EventService.Application.EventManagement.Common;
using EventService.Application.Persistence;
using EventService.Application.Services;
using EventService.Domain.EventAggregate;
using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;
using MediatR;

namespace EventService.Application.EventManagement.Command.CreateEventCommand;

public class CreateEventCommandHandler
    : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    private readonly IEventService _eventService;
    private readonly IEventTypeService _eventTypeService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventCommandHandler(
        IEventService eventService,
        IEventTypeService eventTypeService,
        IUnitOfWork unitOfWork)
    {
        _eventService = eventService;
        _eventTypeService = eventTypeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateEventResult> Handle(
        CreateEventCommand request,
        CancellationToken cancellationToken)
    {
        var eventExists = await _eventService
            .CheckEventNotExists(
                request.Title, 
                UserId.Create(request.OrganizerId), 
                cancellationToken);

        if (eventExists)
            throw new InvalidOperationException("Event already exists");

        EventType eventType;

        if (request.EventTypeId.HasValue)
        {
            eventType = await _eventTypeService
            .GetEventTypeByIdAsync(
                EventTypeId.Create(request.EventTypeId.Value), cancellationToken)
            ?? throw new EntityNotFoundException(
                $"EventType:{request.EventTypeId} not found");
        }
        else
        {
            eventType = await _eventTypeService.GetDefaultEventTypeAsync(cancellationToken)
            ?? throw new EntityNotFoundException("Default EventType not found");
        }

        var newEvent = Event.Create(
            request.Title,
            request.Description!,
            eventTypeId: eventType.Id,
            request.Location!,
            request.StartDate,
            request.EndDate,
            request.MaxParticipants,
            UserId.Create(request.OrganizerId),
            TimeProvider.System.GetUtcNow().DateTime,
            TimeProvider.System.GetUtcNow().DateTime
        );

        await _eventService.CreateEventAsync(newEvent, cancellationToken);

        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return new CreateEventResult(
                    newEvent.Id.Value,
                    newEvent.CreatedAt,
                    newEvent.UpdatedAt,
                    newEvent.Status.Id);
    }
}
