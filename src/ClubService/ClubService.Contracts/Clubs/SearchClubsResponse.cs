namespace ClubService.Contracts.Clubs;

public record SearchClubsResponse(
    Guid Id,
    string Name,
    string Description,
    IEnumerable<string> Interests,
    Guid OwnerId);