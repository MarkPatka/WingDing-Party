namespace UserService.Contracts.Avatars;

public record CreateAvatarRequest(
    Stream AvatarStream,
    string FileName,
    string ContentType,
    Guid UserId,
    bool IsDefault,
    bool IsActive);