using ClubService.Application.ClubManagement.Common;
using MediatR;

namespace ClubService.Application.ClubManagement.Command.LeaveClubCommand;

public record LeaveClubCommand(Guid  UserId, Guid ClubId) :  IRequest<LeaveClubResult>;