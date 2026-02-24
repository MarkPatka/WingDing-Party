using ClubService.Application.ClubManagement.Common;
using MediatR;

namespace ClubService.Application.ClubManagement.Queries.GetClubMembersQuery;

public record GetClubMembersQuery(Guid Id) : IRequest<IEnumerable<GetClubMembersResult>>;