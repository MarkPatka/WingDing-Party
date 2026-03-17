using ClubService.Application.ClubManagement.Common;
using ClubService.Application.Common.Exceptions;
using ClubService.Application.Persistence;
using ClubService.Application.Services;
using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.ValueObjects;
using MediatR;

namespace ClubService.Application.ClubManagement.Command.CreateClubCommand;

public class CreateClubCommandHandler : IRequestHandler<CreateClubCommand, CreateClubResult>
{
    private readonly IClubService _clubService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateClubCommandHandler(IClubService clubService, IUnitOfWork unitOfWork)
    {
        _clubService = clubService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateClubResult> Handle(CreateClubCommand request, CancellationToken cancellationToken)
    {
        bool isExist = await _clubService
            .GetAnyClubsAsync(request.Name, null, OwnerId.Create(request.OwnerId));

        if (isExist)
        {
            throw new EntityNotFoundException("The same club exists");
        }

        var club = Club.Create(
            request.Name,
            request.Description,
            request.Interests,
            OwnerId.Create(request.OwnerId),
            request.IsPublic);

        club = await _clubService.AddAsync(club).ConfigureAwait(true);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return await Task.FromResult(
            new CreateClubResult(
                club.Id.Value,
                club.Name,
                club.Description,
                club.Owner.Value,
                club.IsPublic)
        );
    }
}