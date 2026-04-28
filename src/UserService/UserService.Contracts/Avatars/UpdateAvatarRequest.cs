namespace UserService.Contracts.Avatars;

public record UpdateAvatarRequest(
    Uri? Avatar,
    string UserId,
    bool IsDefault,
    bool IsActive);