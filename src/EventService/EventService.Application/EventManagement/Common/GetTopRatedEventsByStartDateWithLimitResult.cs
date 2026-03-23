using EventService.Contracts.DTO;

namespace EventService.Application.EventManagement.Common;

public record GetTopRatedEventsByStartDateWithLimitResult(
    IEnumerable<EventDto> Events);
