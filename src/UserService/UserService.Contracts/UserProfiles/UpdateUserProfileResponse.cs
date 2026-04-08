namespace UserService.Contracts.UserProfiles;

public record UpdateUserProfileResponse(
    string DisplayName,
    string Bio,
    Uri? AvatarUri,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);