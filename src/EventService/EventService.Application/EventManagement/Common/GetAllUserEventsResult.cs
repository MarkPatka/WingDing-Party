using EventService.Contracts.DTO;

namespace EventService.Application.EventManagement.Common;

public record GetAllUserEventsResult(
    IEnumerable<EventDto> Events);
