namespace ClubService.Application.ClubManagement.Common;

public record GetClubsByUserResult(Guid Id, string Name, string Description, Guid OwnerId, bool IsPublic);