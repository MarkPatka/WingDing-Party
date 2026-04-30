namespace UserService.Contracts.Avatars;

public record CreateAvatarResponse(Guid UserId, Uri? Avatar, bool IsDefault, bool IsActive);