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

    public GetEventsByTextAndFiltersQueryHandler(
        IEventService service)
    {
        _eventService = service;
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

        var eventDtos = pagedEvents.Items.Select(e => new EventDto(
            e.Id.Value.ToString(),
            e.Title,
            e.Description,
            e.EventType is not null
                ? new EventTypeDto(
                    e.EventType.Id.Value.ToString(),
                    e.EventType.Name,
                    e.EventType.Description,
                    e.EventType.Icon)
                : new EventTypeDto("unknown", "unknown", null, null),
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
