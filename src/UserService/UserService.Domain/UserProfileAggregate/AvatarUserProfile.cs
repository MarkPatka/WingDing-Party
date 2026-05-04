using UserService.Domain.UserProfileAggregate.Entities;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Domain.UserProfileAggregate;

public sealed partial class UserProfile
{
    public AvatarId AddAvatar(Uri avatarUri, UserId userId, bool isActive, bool isDefault)
    {
        var avatar = Avatar.Create(AvatarPath.Create(avatarUri), userId, isDefault, isActive, DateTime.UtcNow);
        Avatars.Add(avatar);
        
        SetDefaultAvatar(avatar.Id, isDefault);
        UpdatedAt = DateTime.UtcNow;
        return avatar.Id;
    }

    public Avatar? GetAvatarById(AvatarId avatarId)
    {
        return Avatars.GetById(avatarId);
    }

    public void UpdateAvatar(Avatar avatar, bool isActive, bool isDefault)
    {
        avatar.Update(isActive, isDefault);
        Avatars.Update(avatar);
    }

    public void RemoveAvatar(AvatarId avatarId)
    {
        Avatars.Remove(avatarId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDefaultAvatar(AvatarId avatarId, bool isDefault)
    {
        Avatars.SetDefault(avatarId, isDefault);
        UpdatedAt = DateTime.UtcNow;
    }
}