namespace UserService.Application.UserProfileManagement.Common;

public record GetUserProfileResult(
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate,
    IReadOnlyList<GetUserProfileAvatarResult> Avatars
);

public record GetUserProfileAvatarResult(Guid AvatarId, Uri AvatarUri, bool IsDefault, bool IsActive);