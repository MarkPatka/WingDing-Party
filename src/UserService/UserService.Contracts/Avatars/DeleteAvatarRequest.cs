namespace UserService.Contracts.Avatars;

public record DeleteAvatarRequest(
    Uri? Avatar,
    Guid UserId);