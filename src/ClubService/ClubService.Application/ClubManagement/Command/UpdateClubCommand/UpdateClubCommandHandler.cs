using ClubService.Application.ClubManagement.Common;
using ClubService.Application.Common.Exceptions;
using ClubService.Application.Persistence;
using ClubService.Application.Services;
using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.ValueObjects;
using MediatR;

namespace ClubService.Application.ClubManagement.Command.UpdateClubCommand;

public class UpdateClubCommandHandler : IRequestHandler<UpdateClubCommand, UpdateClubResult>
{
    private readonly IClubService _clubService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClubCommandHandler(IClubService clubService, IUnitOfWork unitOfWork)
    {
        _clubService = clubService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateClubResult> Handle(UpdateClubCommand request, CancellationToken cancellationToken)
    {
        Club? club = await _clubService.GetClubByIdAsync(ClubId.Create(request.ClubId));
        if (club == null)
        {
            throw new EntityNotFoundException("The club doesn't exist");
        }

        club.Update(
            request.Name,
            request.Description,
            request.Interests,
            OwnerId.Create(request.OwnerId),
            request.IsPublic);

        await _clubService.UpdateAsync(club).ConfigureAwait(true);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return await Task.FromResult(
            new UpdateClubResult(
                club.Id.Value,
                club.Name,
                club.Description,
                club.Interests,
                club.Owner.Value,
                club.IsPublic)
        );
    }
}