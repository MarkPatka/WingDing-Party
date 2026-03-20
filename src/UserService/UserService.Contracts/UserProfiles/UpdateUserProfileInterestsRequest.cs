namespace UserService.Contracts.UserProfiles;

public record UpdateUserProfileInterestsRequest(Guid UserId, IEnumerable<string> Interests);