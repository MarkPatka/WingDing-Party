using ClubService.Application.ClubManagement.Common;
using MediatR;

namespace ClubService.Application.ClubManagement.Command.DeleteClubCommand;

public record DeleteClubCommand(Guid Id) : IRequest<DeleteClubResult>;