namespace UserService.Contracts.Avatars;

public record UpdateAvatarResponse(
    Uri? Avatar,
    string UserId,
    bool IsDefault,
    bool IsActive);