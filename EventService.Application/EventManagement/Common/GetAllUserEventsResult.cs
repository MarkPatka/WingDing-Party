using EventService.Domain;

namespace EventService.Application.EventManagement.Common;

public record GetAllUserEventsResult(
    IEnumerable<Event> Events);
