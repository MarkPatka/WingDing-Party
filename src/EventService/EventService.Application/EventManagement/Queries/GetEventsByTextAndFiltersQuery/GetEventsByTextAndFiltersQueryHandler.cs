using EventService.Application.Common.Exceptions;
using EventService.Application.EventManagement.Common;
using EventService.Application.Services;
using EventService.Contracts.DTO;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetEventsByTextAndFiltersQuery;

public class GetEventsByTextAndFiltersQueryHandler
    : IRequestHandler<GetEventsByTextAndFiltersQuery, GetEventsByTextAndFiltersResult>
{
    private readonly IEventService _eventService;
    private readonly IEventTypeService _eventTypeService;

    public GetEventsByTextAndFiltersQueryHandler(
        IEventService service,
        IEventTypeService eventTypeService)
    {
        _eventService = service;
        _eventTypeService = eventTypeService;
    }

    public async Task<GetEventsByTextAndFiltersResult> Handle(
        GetEventsByTextAndFiltersQuery request, CancellationToken cancellationToken)
    {
        var pagedEvents = await _eventService.GetEventsByTextAndFiltersAsync(
            request.Text,
            request.City,
            request.DateFrom,
            request.DateTo,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        if (!pagedEvents.Items.Any())
            throw new EntityNotFoundException(
                $"Events by text '{request.Text}' not found");

        var eventTypes = new EventTypeDto?[pagedEvents.Items.Count];
        for (int i = 0; i < pagedEvents.Items.Count; i++)
        {
            var eventType = await _eventTypeService.GetEventTypeByIdAsync(
                pagedEvents.Items[i].EventTypeId, cancellationToken);

            eventTypes[i] = eventType is not null
                ? new EventTypeDto(
                    eventType.Id.Value.ToString(),
                    eventType.Name,
                    eventType.Description,
                    eventType.Icon)
                : null;
        }

        var eventDtos = pagedEvents.Items.Zip(eventTypes, (e, et) => new EventDto(
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

        return new GetEventsByTextAndFiltersResult(
            eventDtos,
            pagedEvents.TotalCount,
            pagedEvents.PageNumber,
            pagedEvents.PageSize,
            pagedEvents.TotalPages,
            pagedEvents.HasNext,
            pagedEvents.HasPrevious);
    }
}
