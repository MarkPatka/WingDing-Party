using MediatR;
using UserService.Application.AvatarManagement.Common;
using UserService.Application.Common.Exceptions;
using UserService.Application.Persistence;
using UserService.Application.Services;
using UserService.Domain.Common.Exceptions;
using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.Entities;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.AvatarManagement.Commands.DeleteAvatarCommand;

public class DeleteAvatarCommandHandler : IRequestHandler<DeleteAvatarCommand, DeleteAvatarResult>
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorage _fileStorage;

    public DeleteAvatarCommandHandler(IUserProfileService userProfileService, IUnitOfWork unitOfWork,
        IFileStorage fileStorage)
    {
        _userProfileService = userProfileService;
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
    }

    public async Task<DeleteAvatarResult> Handle(DeleteAvatarCommand request, CancellationToken cancellationToken)
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

        userProfile.RemoveAvatar(avatar.Id);

        try
        {
            await _userProfileService.UpdateAsync(userProfile, cancellationToken);
            await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new AvatarException("Avatar wasn't deleted ", ex);
        }

        await _fileStorage.DeleteAsync(avatar.AvatarPath.Value);

        return new DeleteAvatarResult();
    }
}