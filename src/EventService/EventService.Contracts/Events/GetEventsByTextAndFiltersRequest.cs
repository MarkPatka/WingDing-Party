namespace EventService.Contracts.Events;

public sealed record GetEventsByTextAndFiltersRequest(
    string Text,
    string? City,
    DateTime? DateFrom,
    DateTime? DateTo,
    int PageNumber = 1,
    int PageSize = 20);