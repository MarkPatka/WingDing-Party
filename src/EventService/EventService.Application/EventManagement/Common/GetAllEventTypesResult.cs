using EventService.Contracts.DTO;

namespace EventService.Application.EventManagement.Common;

public record GetAllEventTypesResult(
    IEnumerable<EventTypeDto> EventTypes);
