namespace ClubService.Contracts.Clubs;

public record CreateClubResponse(string Id, string Name, string Description, string OwnerId, bool IsPublic);