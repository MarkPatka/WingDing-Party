using FluentValidation;

namespace ClubService.Application.ClubManagement.Command.JoinClubCommand;

public class JoinToClubCommandValidator : AbstractValidator<JoinToClubCommand>
{
    public JoinToClubCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ClubId).NotEmpty();
    }
}