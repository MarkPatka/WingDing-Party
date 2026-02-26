using ClubService.Application.ClubManagement.Common;
using MediatR;

namespace ClubService.Application.ClubManagement.Queries.GetClubsByUserQuery;

public record GetClubsByUserQuery(Guid UserId) : IRequest<IEnumerable<GetClubsByUserResult>>;