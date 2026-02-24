using ClubService.Application.ClubManagement.Common;
using MediatR;
namespace ClubService.Application.ClubManagement.Queries.GetClubQuery;

public record GetClubQuery(Guid ClubId) : IRequest<GetClubResult>;