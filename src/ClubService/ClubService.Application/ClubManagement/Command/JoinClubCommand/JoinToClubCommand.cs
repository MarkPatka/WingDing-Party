using ClubService.Application.ClubManagement.Common;
using MediatR;

namespace ClubService.Application.ClubManagement.Command.JoinClubCommand;

public record JoinToClubCommand(Guid UserId, Guid ClubId) :  IRequest<JoinToClubResult>;