using FluentValidation;

namespace ClubService.Application.ClubManagement.Command.DeleteClubCommand;

public class DeleteClubCommandValidator : AbstractValidator<DeleteClubCommand>
{
    public DeleteClubCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}