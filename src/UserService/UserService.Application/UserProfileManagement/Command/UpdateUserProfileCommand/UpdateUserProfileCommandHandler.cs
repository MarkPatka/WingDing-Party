using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Application.Persistence;
using UserService.Application.Services;
using UserService.Application.UserProfileManagement.Common;
using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.UserProfileManagement.Command.UpdateUserProfileCommand;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UpdateUserProfileResult>
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(IUserProfileService userProfileService, IUnitOfWork unitOfWork)
    {
        _userProfileService = userProfileService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateUserProfileResult> Handle(UpdateUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        UserProfile? userProfile = await _userProfileService.GetUserProfileByIdAsync(UserId.Create(request.Id))
            ;

        if (userProfile == null)
        {
            throw new EntityNotFoundException("UserProfile doesn't exist");
        }

        userProfile.Update(
            request.DisplayName,
            request.Bio,
            request.Interests,
            request.BirthDate
        );

        await _userProfileService.UpdateAsync(userProfile);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        return new UpdateUserProfileResult(
            userProfile.DisplayName,
            userProfile.Bio,
            userProfile.Interests,
            userProfile.BirthDate);
    }
}