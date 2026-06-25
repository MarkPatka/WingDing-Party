namespace ClubService.Application.ClubManagement.Common;

public record SearchClubsResult(Guid Id, string Name, string Description, IEnumerable<string> Interests, Guid OwnerId);