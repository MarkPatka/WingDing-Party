using EventService.Application.EventManagement.Common;
using MediatR;
using EventService.Application.Services;
using EventService.Domain.EventAggregate.ValueObjects;
using EventService.Contracts.DTO;
using EventService.Application.Common.Exceptions;
using EventService.Domain.EventAggregate.Entities;

namespace EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;

public class GetAllUserEventsQueryHandler
    : IRequestHandler<GetAllUserEventsQuery, GetAllUserEventsResult>
{
    private readonly IEventService _eventService;
    private readonly IEventTypeService _eventTypeService;

    public GetAllUserEventsQueryHandler(
        IEventService service,
        IEventTypeService eventTypeService)
    {
        _eventService = service;
        _eventTypeService = eventTypeService;
    }

    public async Task<GetAllUserEventsResult> Handle(
        GetAllUserEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await _eventService.GetEventsByOrganizerIdAsync(
            UserId.Create(request.UserId),
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        if (events == null)
            throw new EntityNotFoundException(
                $"Events by OrganizerId {request.UserId} not found");

        var eventTypes = new EventTypeDto?[events.Count];
        for (int i = 0; i < events.Count; i++)
        {
            var eventType = await _eventTypeService.GetEventTypeByIdAsync(
                events[i].EventTypeId, cancellationToken);

            eventTypes[i] = eventType is not null
                ? new EventTypeDto(
                    eventType.Id.Value.ToString(), 
                    eventType.Name, 
                    eventType.Description, 
                    eventType.Icon)
                : null;
        }

        var eventDtos = events.Zip(eventTypes, (e, et) => new EventDto(
            e.Id.Value.ToString(),
            e.Title,
            e.Description,
            et!,
            e.Status.Name,
            new LocationDto(
                e.Location.Address,
                e.Location.City,
                e.Location.Country),
            e.StartDate,
            e.EndDate,
            e.MaxParticipants
            ));

        return new GetAllUserEventsResult(eventDtos);
    }
}
