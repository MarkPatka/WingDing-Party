using UserService.Domain.Common.Abstract;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Domain.UserProfileAggregate.Entities;

public class Avatar : Entity<AvatarId>
{
    public AvatarPath AvatarPath { get; private set; }
    public UserId UserId { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }


    private Avatar()
    {
    }

    private Avatar(AvatarPath avatarPath, UserId userId, bool isDefault, bool isActive, DateTime createdAt)
    {
        Id = AvatarId.CreateUnique();
        AvatarPath = avatarPath;
        UserId = userId;
        IsActive = isActive;
        IsDefault = isDefault;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public static Avatar Create(AvatarPath avatarPath, UserId userId, bool isDefault, bool isActive, DateTime createdAt)
    {
        Avatar avatar = new(avatarPath, userId, isDefault, isActive, createdAt);
        return avatar;
    }

    public static Avatar Create(AvatarPath avatarPath, UserId userId)
    {
        Avatar avatar = new(avatarPath, userId, true, true, DateTime.UtcNow);
        return avatar;
    }

    public void Update(bool isActive, bool isDefault)
    {
        SetActive(isActive);
        SetDefault(isDefault);
    }

    private void SetActive(bool isActive = true)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetDefault(bool isDefault)
    {
        IsDefault = isDefault;
        UpdatedAt = DateTime.UtcNow;
    }
}