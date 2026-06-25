using EventService.Application.EventManagement.Common;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetEventsByTextAndFiltersQuery;

public record GetEventsByTextAndFiltersQuery(
    string Text,
    string? City,
    DateTime? DateFrom,
    DateTime? DateTo,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<GetEventsByTextAndFiltersResult>;
