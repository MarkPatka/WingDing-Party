using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Application.Services;
using UserService.Application.UserProfileManagement.Common;
using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.UserProfileManagement.Queries.GetUserProfileQuery;

public class GetUserProfileQueryHandler
    : IRequestHandler<GetUserProfileQuery, GetUserProfileResult>
{
    private readonly IUserProfileService _userProfileService;

    public GetUserProfileQueryHandler(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public async Task<GetUserProfileResult> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        UserProfile? userProfile = await _userProfileService.GetUserProfileByIdAsync(UserId.Create(request.UserId))
            ;

        if (userProfile == null)
        {
            throw new EntityNotFoundException("UserProfile doesn't exist");
        }

        var avatars = userProfile.GetAvatars().Select(c =>
                new GetUserProfileAvatarResult(c.Id.Value, c.AvatarPath.Value, c.IsDefault, c.IsActive)).ToList()
            .AsReadOnly();

        return await Task.FromResult(
            new GetUserProfileResult(
                userProfile.DisplayName,
                userProfile.Bio,
                userProfile.Interests,
                userProfile.BirthDate,
                avatars
            ));
    }
}