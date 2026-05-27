namespace EventService.Contracts.Events.Requests;

public sealed record GetAllUserEventsRequest(
    Guid UserId, 
    int PageNumber = 1, 
    int PageSize = 20);
