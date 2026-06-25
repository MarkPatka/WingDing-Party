namespace UserService.Contracts.Avatars;

public record DeleteAvatarRequest(
    Guid AvatarId,
    Guid UserId);