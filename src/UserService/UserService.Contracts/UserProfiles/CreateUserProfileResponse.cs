namespace UserService.Contracts.UserProfiles;

public record CreateUserProfileResponse(
    Guid Id,
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);