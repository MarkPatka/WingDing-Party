using EventService.Application.EventManagement.Common;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetEventByIdQuery;

public record GetEventByIdQuery(Guid EventId) : IRequest<GetEventByIdResult>;
