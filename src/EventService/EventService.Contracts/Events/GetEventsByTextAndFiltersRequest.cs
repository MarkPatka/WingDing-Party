namespace EventService.Contracts.Events;

public sealed record GetEventsByTextAndFiltersRequest(
    string Text,
    string? EventType,
    string? City,
    DateTime? DateFrom,
    DateTime? DateTo,
    int PageNumber,
    int PageSize);