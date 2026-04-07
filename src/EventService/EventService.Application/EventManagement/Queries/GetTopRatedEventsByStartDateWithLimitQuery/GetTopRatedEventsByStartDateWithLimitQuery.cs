using EventService.Application.EventManagement.Common;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetTopRatedEventsByStartDateWithLimitQuery;

public record GetTopRatedEventsByStartDateWithLimitQuery(
    DateTime StartDate, int Limit = 10) 
    : IRequest<GetTopRatedEventsByStartDateWithLimitResult>;
