using EventService.Contracts.DTO;

namespace EventService.Contracts.Events.Responses;

public sealed record GetAllEventTypesResponse(IEnumerable<EventTypeDto> EventTypes);
