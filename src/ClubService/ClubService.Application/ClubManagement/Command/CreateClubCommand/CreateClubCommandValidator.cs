using FluentValidation;

namespace ClubService.Application.ClubManagement.Command.CreateClubCommand;

public class CreateClubCommandValidator : AbstractValidator<ClubService.Application.ClubManagement.Command.CreateClubCommand.CreateClubCommand>
{
    public CreateClubCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}