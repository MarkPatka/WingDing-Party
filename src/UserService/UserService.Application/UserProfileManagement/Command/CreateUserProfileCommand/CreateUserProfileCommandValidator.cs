using FluentValidation;

namespace UserService.Application.UserProfileManagement.Command.CreateUserProfileCommand;

public class CreateUserProfileCommandValidator : AbstractValidator<CreateUserProfileCommand>
{
    public CreateUserProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty();
    }
}