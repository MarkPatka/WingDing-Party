using ClubService.Application.ClubManagement.Common;
using ClubService.Application.Common.Exceptions;
using ClubService.Application.Persistence;
using ClubService.Application.Services;
using ClubService.Domain.ClubAggregate.ValueObjects;
using MediatR;

namespace ClubService.Application.ClubManagement.Command.JoinClubCommand;

public class JoinToClubCommandHandler : IRequestHandler<JoinToClubCommand, JoinToClubResult>
{
    private readonly IClubService _clubService;
    private readonly IUnitOfWork _unitOfWork;

    public JoinToClubCommandHandler(IClubService clubService, IUnitOfWork unitOfWork)
    {
        _clubService = clubService;
        _unitOfWork = unitOfWork;
    }

    public async Task<JoinToClubResult> Handle(JoinToClubCommand request, CancellationToken cancellationToken)
    {
        var clubId = ClubId.Create(request.ClubId);
        var club = await _clubService.GetClubByIdAsync(clubId);
        if (club == null)
        {
            throw new EntityNotFoundException("Club not found");
        }

        var joinedAt = DateTime.UtcNow;
        await _clubService.AddMember(club, UserId.Create(request.UserId), joinedAt);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        return await Task.FromResult(new JoinToClubResult(request.ClubId, request.UserId, joinedAt));
    }
}