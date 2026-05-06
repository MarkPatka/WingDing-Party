using MediatR;
using UserService.Application.AvatarManagement.Common;
using UserService.Application.Common.Exceptions;
using UserService.Application.Persistence;
using UserService.Application.Services;
using UserService.Domain.Common.Exceptions;
using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.Entities;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.AvatarManagement.Commands.UpdateAvatarCommand;

public class UpdateAvatarCommandHandler : IRequestHandler<UpdateAvatarCommand, UpdateAvatarResult>
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAvatarCommandHandler(IUserProfileService userProfileService, IUnitOfWork unitOfWork)
    {
        _userProfileService = userProfileService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateAvatarResult> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        UserProfile? userProfile = await _userProfileService.GetUserProfileByIdAsync(UserId.Create(request.UserId));

        if (userProfile == null)
        {
            throw new EntityNotFoundException("UserProfile not found");
        }

        Avatar? avatar = userProfile.GetAvatarById(AvatarId.Create(request.AvatarId));
        if (avatar is null)
        {
            throw new AvatarNotFoundException("Avatar is not found");
        }

        if (avatar.UserId != UserId.Create(request.UserId))
        {
            throw new AvatarMismatchException("UserId doesn't match to avatars' userId");
        }

        userProfile.UpdateAvatar(avatar, request.IsActive, request.IsDefault);

        try
        {
            await _userProfileService.UpdateAsync(userProfile, cancellationToken);
            await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new AvatarException("Avatar wasn't saved ", ex);
        }

        await _userProfileService.UpdateAsync(userProfile, cancellationToken);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return new UpdateAvatarResult(
            avatar.Id.Value,
            request.UserId,
            avatar.AvatarPath.Value,
            request.IsDefault,
            request.IsActive
        );
    }
}