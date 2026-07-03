using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Application.Persistence;
using UserService.Application.Services;
using UserService.Application.UserProfileManagement.Common;
using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.UserProfileManagement.Command.UpdateUserProfileInterestsCommand;

public class UpdateUserProfileInterestsCommandHandler 
    : IRequestHandler<UpdateUserProfileInterestsCommand, UpdateUserProfileInterestsResult>
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateUserProfileInterestsCommandHandler(IUserProfileService userProfileService, IUnitOfWork unitOfWork)
    {
        _userProfileService = userProfileService;
        _unitOfWork = unitOfWork;
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
            request.Interests,
            userProfile.BirthDate
        );

        await _userProfileService.UpdateAsync(userProfile);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        
        return new UpdateUserProfileInterestsResult(userProfile.Interests);
    }
}