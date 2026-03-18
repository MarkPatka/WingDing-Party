namespace EventService.Contracts.Events;

public sealed record GetAllUserEventsRequest(
    string UserId, 
    int PageNumber = 1, 
    int PageSize = 20);
