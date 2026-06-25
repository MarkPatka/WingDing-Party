namespace EventService.Contracts.Events.Requests;

public sealed record GetTopRatedEventsByStartDateWithLimitRequest(
    DateTime StartDate, int Limit = 10);
