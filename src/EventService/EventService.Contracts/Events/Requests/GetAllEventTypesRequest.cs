namespace EventService.Contracts.Events.Requests;

public sealed record GetAllEventTypesRequest(
    int PageNumber = 1,
    int PageSize = 20);
