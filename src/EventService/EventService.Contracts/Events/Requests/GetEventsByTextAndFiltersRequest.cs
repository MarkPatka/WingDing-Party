namespace EventService.Contracts.Events.Requests;

public sealed record GetEventsByTextAndFiltersRequest(
    string Text,
    string? City,
    DateTime? DateFrom,
    DateTime? DateTo,
    int PageNumber = 1,
    int PageSize = 20);