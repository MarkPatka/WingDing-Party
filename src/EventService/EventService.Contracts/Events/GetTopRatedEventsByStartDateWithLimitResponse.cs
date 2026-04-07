using EventService.Contracts.DTO;

namespace EventService.Contracts.Events;

public sealed record GetTopRatedEventsByStartDateWithLimitResponse(
    IEnumerable<EventDto> Events);
