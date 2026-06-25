namespace UserService.Contracts.UserProfiles;

public record GetUserProfileResponse(
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate,
    IReadOnlyList<GetUserProfileAvatarResponse> Avatars
);

public record GetUserProfileAvatarResponse(Guid AvatarId, Uri AvatarUri, bool IsDefault, bool IsActive);