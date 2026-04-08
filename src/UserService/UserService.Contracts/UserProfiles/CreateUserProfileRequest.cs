namespace UserService.Contracts.UserProfiles;

public record CreateUserProfileRequest(
    string DisplayName,
    string Bio,
    Uri? AvatarUri,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);