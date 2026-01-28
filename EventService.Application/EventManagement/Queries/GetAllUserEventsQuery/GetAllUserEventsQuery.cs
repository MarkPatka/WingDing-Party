using EventService.Application.EventManagement.Common;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;

public record GetAllUserEventsQuery(Guid OrganizerId, int PageNumber, int PageSize) 
    : IRequest<GetAllUserEventsResult>;
