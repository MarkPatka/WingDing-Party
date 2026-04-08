using EventService.Application.EventManagement.Common;
using MediatR;

namespace EventService.Application.EventManagement.Queries.GetAllEventTypesQuery;

public record GetAllEventTypesQuery(
    int PageNumber = 1,
    int PageSize = 20) 
    : IRequest<GetAllEventTypesResult>;
