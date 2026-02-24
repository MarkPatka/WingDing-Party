namespace ClubService.Application.ClubManagement.Common;

public record CreateClubResult(Guid Id, string Name, string Description, Guid OwnerId, bool IsPublic);