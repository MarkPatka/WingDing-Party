namespace UserService.Contracts.Avatars;

public record UpdateAvatarRequest(
    Guid AvatarId,
    Guid UserId,
    bool IsDefault,
    bool IsActive);