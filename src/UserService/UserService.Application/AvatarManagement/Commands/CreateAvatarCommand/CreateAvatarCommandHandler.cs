using MediatR;
using Microsoft.Extensions.Options;
using UserService.Application.AvatarManagement.Common;
using UserService.Application.Common.Configuration;
using UserService.Application.Common.Exceptions;
using UserService.Application.Persistence;
using UserService.Application.Services;
using UserService.Domain.UserProfileAggregate;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Application.AvatarManagement.Commands.CreateAvatarCommand;

public class CreateAvatarCommandHandler : IRequestHandler<CreateAvatarCommand, CreateAvatarResult>
{
    private readonly IUserProfileService _userProfileService;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptionsMonitor<FileStorageOptions> _fileStorageOptions;

    public CreateAvatarCommandHandler(IUserProfileService userProfileService, IUnitOfWork unitOfWork,
        IFileStorage fileStorage, IOptionsMonitor<FileStorageOptions> fileStorageOptions)
    {
        _userProfileService = userProfileService;
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
        _fileStorageOptions = fileStorageOptions;
    }

    public async Task<CreateAvatarResult> Handle(CreateAvatarCommand request,
        CancellationToken cancellationToken)
    {
        UserProfile? userProfile = await _userProfileService.GetUserProfileByIdAsync(UserId.Create(request.UserId));

        if (userProfile == null)
        {
            throw new EntityNotFoundException("UserProfile not found");
        }

        var avatarUri = await _fileStorage.SaveAsync(
            request.AvatarStream,
            request.FileName,
            _fileStorageOptions.CurrentValue.AvatarBucket,
            request.ContentType);

        if (avatarUri is null)
        {
            throw new ArgumentNullException("Avatar wasn't uploaded");
        }

        var avatarId =
            userProfile.AddAvatar(avatarUri, UserId.Create(request.UserId), request.IsActive, request.IsDefault);
        try
        {
            await _userProfileService.UpdateAsync(userProfile, cancellationToken);
            await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        catch (Exception exSaved)
        {
            try
            {
                await _fileStorage.DeleteAsync(avatarUri, cancellationToken);
            }
            catch (Exception exStorage)
            {
                throw new AvatarException("Avatar wasn't removed ", exStorage);
            }

            throw new AvatarException("Avatar wasn't saved ", exSaved);
        }

        return new CreateAvatarResult(
            avatarId.Value,
            request.UserId,
            avatarUri,
            request.IsDefault,
            request.IsActive
        );
    }
}