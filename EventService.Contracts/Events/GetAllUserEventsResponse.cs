using EventService.Domain;

namespace EventService.Contracts.Events;

public sealed record GetAllUserEventsResponse(IEnumerable<Event> Events);
