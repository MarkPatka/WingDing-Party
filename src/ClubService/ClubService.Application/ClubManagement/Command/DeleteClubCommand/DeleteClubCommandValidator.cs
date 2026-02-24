using FluentValidation;

namespace ClubService.Application.ClubManagement.Command.DeleteClubCommand;

public class DeleteClubCommandValidator : AbstractValidator<ClubService.Application.ClubManagement.Command.DeleteClubCommand.DeleteClubCommand>
{
    public DeleteClubCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}