namespace ClubService.Contracts.Clubs;

public record UpdateClubResponse(
    Guid ClubId,
    string Name,
    string Description,
    IEnumerable<string> Interests,
    Guid OwnerId,
    bool IsPublic);