using ClubService.Application.ClubManagement.Common;
using ClubService.Application.Common.Exceptions;
using ClubService.Application.Persistence;
using ClubService.Application.Services;
using ClubService.Domain.ClubAggregate;
using ClubService.Domain.ClubAggregate.ValueObjects;
using MediatR;

namespace ClubService.Application.ClubManagement.Command.DeleteClubCommand;

public class DeleteClubCommandHandler : IRequestHandler<DeleteClubCommand, DeleteClubResult>
{
    private readonly IClubService _clubService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteClubCommandHandler(IClubService clubService, IUnitOfWork unitOfWork)
    {
        _clubService = clubService;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteClubResult> Handle(DeleteClubCommand request, CancellationToken cancellationToken)
    {
        Club? club = await _clubService.GetClubByIdAsync(ClubId.Create(request.Id));
        if (club == null)
        {
            throw new EntityNotFoundException("The club doesn't exist");
        }

        await _clubService.DeleteAsync(club).ConfigureAwait(true);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        return await Task.FromResult(new DeleteClubResult());
    }
}