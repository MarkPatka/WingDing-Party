using ClubService.Application.ClubManagement.Common;
using MediatR;

namespace ClubService.Application.ClubManagement.Command.UpdateClubCommand;

public record UpdateClubCommand(
    Guid ClubId,
    string Name,
    string Description,
    IEnumerable<string> Interests,
    Guid OwnerId,
    bool IsPublic)
    : IRequest<UpdateClubResult>;