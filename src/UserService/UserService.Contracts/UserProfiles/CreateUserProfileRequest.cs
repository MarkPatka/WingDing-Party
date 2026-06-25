namespace UserService.Contracts.UserProfiles;

public record CreateUserProfileRequest(
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);