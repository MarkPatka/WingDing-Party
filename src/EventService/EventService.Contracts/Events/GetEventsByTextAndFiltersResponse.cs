using EventService.Contracts.DTO;

namespace EventService.Contracts.Events;

public sealed record GetEventsByTextAndFiltersResponse(IEnumerable<EventDto> Events);
