using UserService.Domain.Common.Abstract;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Domain.UserProfileAggregate.Entities;

public class Avatar : Entity<AvatarId>
{
    public UserId UserId { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }


    private Avatar()
    {
    }

    private Avatar(AvatarId value, UserId userId, bool isDefault, bool isActive, DateTime createdAt)
    {
        Id = value;
        UserId = userId;
        IsActive = isActive;
        IsDefault = isDefault;
        CreatedAt = createdAt;
    }

    public static Avatar Create(AvatarId value, UserId userId, bool isDefault, bool isActive, DateTime createdAt)
    {
        Avatar avatar = new(value, userId, isDefault, isActive, createdAt);
        return avatar;
    }

    public static Avatar Create(AvatarId value, UserId userId)
    {
        Avatar avatar = new(value, userId, true, true, DateTime.UtcNow);
        return avatar;
    }

    public void Update(bool isDefault, bool isActive)
    {
        SetDefault(isDefault);
        SetActive(isActive);
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