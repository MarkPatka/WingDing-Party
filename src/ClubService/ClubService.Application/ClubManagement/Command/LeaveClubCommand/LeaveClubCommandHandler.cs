using ClubService.Application.ClubManagement.Common;
using ClubService.Application.Common.Exceptions;
using ClubService.Application.Persistence;
using ClubService.Application.Services;
using ClubService.Domain.ClubAggregate.ValueObjects;
using MediatR;

namespace ClubService.Application.ClubManagement.Command.LeaveClubCommand;

public class LeaveClubCommandHandler : IRequestHandler<LeaveClubCommand, LeaveClubResult>
{
    private readonly IClubService _clubService;
    private readonly IUnitOfWork _unitOfWork;

    public LeaveClubCommandHandler(IClubService clubService, IUnitOfWork unitOfWork)
    {
        _clubService = clubService;
        _unitOfWork = unitOfWork;
    }

    public async Task<LeaveClubResult> Handle(LeaveClubCommand request, CancellationToken cancellationToken)
    {
        var clubId = ClubId.Create(request.ClubId);
        var userId = UserId.Create(request.UserId);
        var club = await _clubService.GetClubByIdAsync(clubId);
        if (club == null)
        {
            throw new EntityNotFoundException("Club not found");
        }

        bool isMemberParticipated = await _clubService
            .IsMemberParticipatedAsync(clubId, userId);
        if (!isMemberParticipated)
        {
            throw new AlreadyDoneException("You have already left the club");
        }

        await _clubService.DeleteMember(club, userId, DateTime.UtcNow);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        return await Task.FromResult(new LeaveClubResult());
    }
}