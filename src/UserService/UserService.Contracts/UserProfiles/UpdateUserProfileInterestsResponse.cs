namespace UserService.Contracts.UserProfiles;

public record UpdateUserProfileInterestsResponse(IEnumerable<string> Interests);