using ClubService.Application.ClubManagement.Command.JoinClubCommand;
using FluentValidation;

namespace ClubService.Application.ClubManagement.Command.LeaveClubCommand;

public class LeaveClubCommandValidator : AbstractValidator<LeaveClubCommand>
{
    public LeaveClubCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ClubId).NotEmpty();
    }
}