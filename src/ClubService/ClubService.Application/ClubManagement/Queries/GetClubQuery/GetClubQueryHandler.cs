using ClubService.Application.ClubManagement.Common;
using ClubService.Application.Common.Exceptions;
using ClubService.Application.Services;
using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.ValueObjects;
using MediatR;

namespace ClubService.Application.ClubManagement.Queries.GetClubQuery;

public class GetClubQueryHandler
    : IRequestHandler<GetClubQuery, GetClubResult>
{
    private readonly IClubService _clubService;

    public GetClubQueryHandler(IClubService clubService)
    {
        _clubService = clubService;
    }

    public async Task<GetClubResult> Handle(GetClubQuery request, CancellationToken cancellationToken)
    {
        Club? club = await _clubService.GetClubByIdAsync(ClubId.Create(request.ClubId));

        if (club == null)
        {
            throw new EntityNotFoundException("Club doesn't exist");
        }

        return await Task.FromResult(
            new GetClubResult(
                club.Id.Value,
                club.Name,
                club.Description,
                club.Owner.Value,
                club.IsPublic)
        );
    }
}