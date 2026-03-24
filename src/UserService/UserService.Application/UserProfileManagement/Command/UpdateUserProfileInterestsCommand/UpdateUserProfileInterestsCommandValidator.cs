using FluentValidation;

namespace UserService.Application.UserProfileManagement.Command.UpdateUserProfileInterestsCommand;

public class
    UpdateUserProfileInterestsCommandValidator : AbstractValidator<
    UpdateUserProfileInterestsCommand>
{
    public UpdateUserProfileInterestsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Interests).IsInEnum();
    }
}