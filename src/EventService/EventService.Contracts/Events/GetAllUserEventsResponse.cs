using EventService.Contracts.DTO;

namespace EventService.Contracts.Events;

public sealed record GetAllUserEventsResponse(IEnumerable<EventDto> Events);
