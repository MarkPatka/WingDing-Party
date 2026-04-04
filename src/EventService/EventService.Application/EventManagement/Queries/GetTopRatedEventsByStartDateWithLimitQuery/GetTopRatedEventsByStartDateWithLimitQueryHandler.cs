using EventService.Application.Common.Exceptions;
using EventService.Application.EventManagement.Common;
using EventService.Application.Services;
using EventService.Contracts.DTO;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetTopRatedEventsByStartDateWithLimitQuery;

public class GetTopRatedEventsByStartDateWithLimitQueryHandler
    : IRequestHandler<GetTopRatedEventsByStartDateWithLimitQuery, GetTopRatedEventsByStartDateWithLimitResult>
{
    private readonly IEventService _eventService;
    private readonly IEventTypeService _eventTypeService;

    public GetTopRatedEventsByStartDateWithLimitQueryHandler(
        IEventService eventService,
        IEventTypeService eventTypeService)
    {
        _eventService = eventService;
        _eventTypeService = eventTypeService;
    }

    public async Task<GetTopRatedEventsByStartDateWithLimitResult> Handle(
        GetTopRatedEventsByStartDateWithLimitQuery request, CancellationToken cancellationToken)
    {
        var events = await _eventService.GetTopRatedEventsByStartDateWithLimitAsync(
            request.StartDate, request.Limit, cancellationToken);

        if (events == null)
            throw new EntityNotFoundException("Events not found");

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
            et! ?? new EventTypeDto("unknown", "unknown", null, null),
            e.Status.Name,
            new LocationDto(
                e.Location.Address,
                e.Location.City,
                e.Location.Country),
            e.StartDate,
            e.EndDate,
            e.MaxParticipants
            ));

        return new GetTopRatedEventsByStartDateWithLimitResult(eventDtos);
    }
}
