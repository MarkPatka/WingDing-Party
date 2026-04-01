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

    public GetTopRatedEventsByStartDateWithLimitQueryHandler(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task<GetTopRatedEventsByStartDateWithLimitResult> Handle(
        GetTopRatedEventsByStartDateWithLimitQuery request, CancellationToken cancellationToken)
    {
        var events = await _eventService.GetTopRatedEventsByStartDateWithLimitAsync(
            request.StartDate, request.Limit, cancellationToken);

        if (events == null)
            throw new EntityNotFoundException("Events not found");

        var eventDtos = events.Select(e => new EventDto(
            e.Id.Value.ToString(),
            e.Title,
            e.Description,
            e.EventTypeId.Value.ToString(),
            e.Status.Name,
            new LocationDto(
                e.Location.Address,
                e.Location.City,
                e.Location.Country),
            e.StartDate,
            e.EndDate,
            e.MaxParticipants));

        return new GetTopRatedEventsByStartDateWithLimitResult(eventDtos);
    }
}
