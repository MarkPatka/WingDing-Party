using MediatR;
using UserService.Application.Services;
using UserService.Application.UserProfileManagement.Common;
using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.UserProfileManagement.Command.UpdateUserProfileInterestsCommand;

public class
    UpdateUserProfileInterestsCommandHandler : IRequestHandler<UpdateUserProfileInterestsCommand,
    UpdateUserProfileInterestsResult>
{
    private readonly IUserProfileService _userProfileService;

    public UpdateUserProfileInterestsCommandHandler(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public async Task<UpdateUserProfileInterestsResult> Handle(UpdateUserProfileInterestsCommand request,
        CancellationToken cancellationToken)
    {
        UserProfile? userProfile =
            await _userProfileService.GetUserProfileByIdAsync(UserId.Create(request.UserId));

        if (userProfile == null)
        {
            throw new EntityNotFoundException("UserProfile doesn't exist");
        }

        userProfile.Update(
            userProfile.DisplayName,
            userProfile.Bio,
            userProfile.AvatarUri,
            request.Interests,
            userProfile.BirthDate
        );

        await _userProfileService.UpdateAsync(userProfile);

        return new UpdateUserProfileInterestsResult(userProfile.Interests);
    }
}