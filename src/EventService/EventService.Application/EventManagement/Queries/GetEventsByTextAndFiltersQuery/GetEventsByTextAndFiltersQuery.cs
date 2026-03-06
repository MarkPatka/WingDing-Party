using EventService.Application.EventManagement.Common;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetEventsByTextAndFiltersQuery;

public record GetEventsByTextAndFiltersQuery(
    string Text,
    string? EventType,
    string? City,
    DateTime? DateFrom,
    DateTime? DateTo,
    int PageNumber,
    int PageSize) : IRequest<GetEventsByTextAndFiltersResult>;
