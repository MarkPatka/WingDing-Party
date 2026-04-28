using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Application.Persistence;
using UserService.Application.Services;
using UserService.Application.UserProfileManagement.Common;
using UserService.Domain.UserProfileAggregate;

namespace UserService.Application.UserProfileManagement.Command.CreateUserProfileCommand;

public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, CreateUserProfileResult>
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorage _fileStorage;

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

        await _userProfileService.InsertAsync(userProfile, cancellationToken);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return new CreateUserProfileResult(
            userProfile.Id.Value,
            userProfile.DisplayName,
            userProfile.Bio,
            //TODO
            userProfile.Avatars.Items.FirstOrDefault(),
            userProfile.Interests,
            userProfile.BirthDate);
    }
}