using FluentValidation;

namespace UserService.Application.UserProfileManagement.Command.CreateUserProfileCommand;

public class CreateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand.UpdateUserProfileCommand>
{
    public CreateUserProfileCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DisplayName).NotEmpty();
    }
}