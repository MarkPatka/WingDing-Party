namespace EventService.Contracts.Events;

public sealed record GetAllUserEventsRequest(string UserId, int PageNumber, int PageSize);
