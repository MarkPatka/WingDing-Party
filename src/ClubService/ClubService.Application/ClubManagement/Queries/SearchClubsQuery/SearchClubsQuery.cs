using ClubService.Application.ClubManagement.Common;
using MediatR;

namespace ClubService.Application.ClubManagement.Queries.SearchClubsQuery;

public record SearchClubsQuery(string Name, IEnumerable<string> Interests) : IRequest<IEnumerable<SearchClubsResult>>;