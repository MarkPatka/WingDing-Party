namespace UserService.Contracts.UserProfiles;

public record UpdateUserProfileResponse(
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);