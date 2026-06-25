using ClubService.Application.ClubManagement.Common;
using ClubService.Application.Services;
using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.Entities;
using ClubService.Domain.ClubAggregate.ValueObjects;
using MediatR;

namespace ClubService.Application.ClubManagement.Queries.GetClubMembersQuery;

public class GetClubMembersHandler
    : IRequestHandler<GetClubMembersQuery, IEnumerable<GetClubMembersResult>>
{
    private readonly IClubService _clubService;

    public GetClubMembersHandler(IClubService clubService)
    {
        _clubService = clubService;
    }

    public async Task<IEnumerable<GetClubMembersResult>> Handle(GetClubMembersQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<ClubMember> clubMembers = await _clubService.GetClubMembersAsync(ClubId.Create(request.Id))
            ;

        return await Task.FromResult(clubMembers.Select(c => new GetClubMembersResult(c.UserId.Value, c.JoinedAt)));
    }
}