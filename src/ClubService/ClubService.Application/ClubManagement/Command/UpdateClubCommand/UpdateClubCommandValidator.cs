using FluentValidation;

namespace ClubService.Application.ClubManagement.Command.UpdateClubCommand;

public class UpdateClubCommandValidator : AbstractValidator<ClubService.Application.ClubManagement.Command.UpdateClubCommand.UpdateClubCommand>
{
    public UpdateClubCommandValidator()
    {
        RuleFor(x => x.ClubId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}