using System.Text.Json.Serialization;

namespace UserService.Contracts.UserProfiles;

public record CreateUserProfileRequest(
    string DisplayName,
    string Bio,
    Uri? Avatar,
    IReadOnlyList<string> Interests,
    DateTime? BirthDate);