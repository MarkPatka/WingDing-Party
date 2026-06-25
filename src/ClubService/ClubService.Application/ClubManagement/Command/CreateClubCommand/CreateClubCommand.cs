using ClubService.Application.ClubManagement.Common;
using MediatR;

namespace ClubService.Application.ClubManagement.Command.CreateClubCommand;

public record CreateClubCommand(string Name, string Description, IEnumerable<string> Interests, Guid OwnerId, bool IsPublic) :  IRequest<CreateClubResult>;