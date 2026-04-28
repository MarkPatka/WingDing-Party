namespace UserService.Contracts.Avatars;

public record CreateAvatarResponse(
    Uri? Avatar,
    bool IsDefault,
    bool IsActive);