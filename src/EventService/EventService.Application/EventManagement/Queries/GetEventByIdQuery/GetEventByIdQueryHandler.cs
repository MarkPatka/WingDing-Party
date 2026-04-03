using EventService.Application.Common.Exceptions;
using EventService.Application.EventManagement.Common;
using EventService.Application.Services;
using EventService.Contracts.DTO;
using EventService.Domain.EventAggregate.ValueObjects;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetEventByIdQuery;

public class GetEventByIdQueryHandler
    : IRequestHandler<GetEventByIdQuery, GetEventByIdResult>
{
    private readonly IEventService _eventService;
    private readonly IEventTypeService _eventTypeService;

    public GetEventByIdQueryHandler(
        IEventService eventService,
        IEventTypeService eventTypeService)
    {
        _eventService = eventService;
        _eventTypeService = eventTypeService;
    }

    public async Task<GetEventByIdResult> Handle(
        GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var @event = await _eventService.GetEventByIdAsync(
            EventId.Create(request.EventId), cancellationToken);

        if (@event is null)
            throw new EntityNotFoundException($"Event {request.EventId} not found");

        var eventType = await _eventTypeService.GetEventTypeByIdAsync(
            @event.EventTypeId, cancellationToken);

        var eventDto = new EventDto(
            @event.Id.Value.ToString(),
            @event.Title,
            @event.Description,
            new EventTypeDto(
                eventType!.Id.Value.ToString(),
                eventType.Name,
                eventType.Description,
                eventType.Icon),
            @event.Status.Name,
            new LocationDto(
                @event.Location.Address,
                @event.Location.City,
                @event.Location.Country),
            @event.StartDate,
            @event.EndDate,
            @event.MaxParticipants
            );

        return new GetEventByIdResult(eventDto);
    }
}
