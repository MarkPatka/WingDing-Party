using UserService.Domain.UserProfileAggregate.Entities;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Domain.UserProfileAggregate;

public sealed partial class UserProfile
{
    public void AddAvatar(Uri avatarUri, bool isActive, bool isDefault)
    {
        Avatars.Add(Avatar.Create(AvatarId.Create(avatarUri)));
        UpdatedAt = DateTime.UtcNow;
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

    public void SetActiveAvatar(AvatarId avatarId, bool isActive)
    {
        Avatars.SetActive(avatarId, isActive);
        UpdatedAt = DateTime.UtcNow;
    }
}