using EventService.Application.EventManagement.Common;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;

public record GetAllUserEventsQuery(
    Guid UserId, 
    int PageNumber = 1, 
    int PageSize = 20) 
    : IRequest<GetAllUserEventsResult>;
