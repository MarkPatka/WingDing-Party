namespace UserService.Contracts.UserProfiles;

public record UpdateUserProfileRequest(
    Guid Id,
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);