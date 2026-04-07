using MediatR;
using UserService.Application.Persistence;
using UserService.Application.Services;
using UserService.Application.UserProfileManagement.Common;
using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.UserProfileManagement.Queries.GetUserProfileInterestsQuery;

public class GetUserProfileQueryInterestsHandler
    : IRequestHandler<GetUserProfileInterestsQuery, GetUserProfileInterestsResult>
{
    private readonly IUserProfileService _userProfileService;

    public GetUserProfileQueryInterestsHandler(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public async Task<GetUserProfileInterestsResult> Handle(GetUserProfileInterestsQuery query,
        CancellationToken cancellationToken)
    {
        UserProfile? userProfile = await _userProfileService.GetUserProfileByIdAsync(UserId.Create(query.UserId))
            ;

        if (userProfile == null)
        {
            throw new Exception("UserProfile doesn't exist");
        }

        return await Task.FromResult(new GetUserProfileInterestsResult(userProfile.Interests));
    }
}