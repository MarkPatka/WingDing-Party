namespace EventService.Contracts.Events;

public sealed record GetAllEventTypesRequest(
    int PageNumber = 1,
    int PageSize = 20);
