using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Contracts.UserProfiles;

public record GetUserProfileInterestsResponse(IEnumerable<string> Interests);