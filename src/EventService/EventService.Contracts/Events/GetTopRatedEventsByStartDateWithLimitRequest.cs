namespace EventService.Contracts.Events;

public sealed record GetTopRatedEventsByStartDateWithLimitRequest(
    DateTime StartDate, int Limit = 10);
