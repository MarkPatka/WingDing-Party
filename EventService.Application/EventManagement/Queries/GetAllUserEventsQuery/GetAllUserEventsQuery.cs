using EventService.Application.EventManagement.Common;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;

public record GetAllUserEventsQuery(string userId) : IRequest<GetAllUserEventsResult>;
