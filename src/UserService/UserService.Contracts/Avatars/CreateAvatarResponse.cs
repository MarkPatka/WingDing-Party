namespace UserService.Contracts.Avatars;

public record CreateAvatarResponse(Guid Id, Guid UserId, Uri? Avatar, bool IsDefault, bool IsActive);