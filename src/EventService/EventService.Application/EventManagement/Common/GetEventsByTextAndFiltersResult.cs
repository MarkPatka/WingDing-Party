using EventService.Contracts.DTO;

namespace EventService.Application.EventManagement.Common;

public record GetEventsByTextAndFiltersResult(IEnumerable<EventDto> Events);
