namespace ClubService.Contracts.Clubs;

public record UpdateClubRequest(
    Guid ClubId,
    string Name,
    string Description,
    IEnumerable<string> Interests,
    Guid OwnerId,
    bool IsPublic);