using EventService.Contracts.DTO;

namespace EventService.Contracts.Events.Responses;

public sealed record GetTopRatedEventsByStartDateWithLimitResponse(
    IEnumerable<EventDto> Events);
