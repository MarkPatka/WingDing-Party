using EventService.Contracts.DTO;

namespace EventService.Contracts.Events.Responses;

public sealed record GetEventsByTextAndFiltersResponse(
    IEnumerable<EventDto> Events,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    bool HasNext,
    bool HasPrevious);
