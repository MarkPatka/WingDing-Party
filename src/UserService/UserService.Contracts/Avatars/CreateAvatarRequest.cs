namespace UserService.Contracts.Avatars;

public record CreateAvatarRequest(
    Uri? Avatar,
    bool IsDefault,
    bool IsActive);