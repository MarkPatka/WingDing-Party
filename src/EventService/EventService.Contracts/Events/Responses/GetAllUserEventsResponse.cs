using EventService.Contracts.DTO;

namespace EventService.Contracts.Events.Responses;

public sealed record GetAllUserEventsResponse(IEnumerable<EventDto> Events);
