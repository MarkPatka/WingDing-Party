using EventService.Contracts.DTO;

namespace EventService.Application.EventManagement.Common;

public record GetEventsByTextAndFiltersResult(
    IEnumerable<EventDto> Events,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    bool HasNext,
    bool HasPrevious);
