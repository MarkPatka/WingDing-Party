using MediatR;
using UserService.Application.Persistence;
using UserService.Application.Services;
using UserService.Application.UserProfileManagement.Common;
using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.UserProfileManagement.Command.CreateUserProfileCommand;

public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, CreateUserProfileResult>
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserProfileCommandHandler(IUserProfileService userProfileService, IUnitOfWork unitOfWork)
    {
        _userProfileService = userProfileService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateUserProfileResult> Handle(CreateUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        UserProfile? userProfile = await _userProfileService.GetUserByNameAsync(request.DisplayName);

        if (userProfile != null)
        {
            throw new EntityAlreadyExistsException("UserProfile already exists");
        }

        userProfile = UserProfile.Create(
            request.DisplayName,
            request.Bio,
            request.AvatarUri,
            request.Interests,
            request.BirthDate);

        await _userProfileService.InsertAsync(userProfile);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return new CreateUserProfileResult(
            userProfile.Id.Value,
            userProfile.DisplayName,
            userProfile.Bio,
            userProfile.AvatarUri,
            userProfile.Interests,
            userProfile.BirthDate);
    }
}