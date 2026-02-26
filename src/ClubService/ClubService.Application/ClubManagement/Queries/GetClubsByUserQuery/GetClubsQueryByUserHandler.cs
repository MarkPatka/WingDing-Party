using ClubService.Application.ClubManagement.Common;
using ClubService.Application.Services;
using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.ValueObjects;
using MediatR;

namespace ClubService.Application.ClubManagement.Queries.GetClubsByUserQuery;

public class GetClubsQueryByUserHandler
    : IRequestHandler<GetClubsByUserQuery, IEnumerable<GetClubsByUserResult>>
{
    private readonly IClubService _clubService;

    public GetClubsQueryByUserHandler(IClubService clubService)
    {
        _clubService = clubService;
    }

    public async Task<IEnumerable<GetClubsByUserResult>> Handle(GetClubsByUserQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Club> clubs = await _clubService.GetClubsByUserAsync(UserId.Create(request.UserId));

        return await Task.FromResult(clubs.Select(c =>
            new GetClubsByUserResult(c.Id.Value, c.Name, c.Description, c.Owner.Value, c.IsPublic)));
    }
}