using EventService.Contracts.DTO;

namespace EventService.Contracts.Events;

public sealed record GetAllEventTypesResponse(IEnumerable<EventTypeDto> EventTypes);
