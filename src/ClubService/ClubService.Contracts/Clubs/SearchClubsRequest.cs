namespace ClubService.Contracts.Clubs;

public record SearchClubsRequest(string Name, IEnumerable<string> Interests);