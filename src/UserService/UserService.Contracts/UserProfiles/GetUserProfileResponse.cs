namespace UserService.Contracts.UserProfiles;

public record GetUserProfileResponse(
    string DisplayName,
    string Bio,
    Uri? AvatarUri,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate
);