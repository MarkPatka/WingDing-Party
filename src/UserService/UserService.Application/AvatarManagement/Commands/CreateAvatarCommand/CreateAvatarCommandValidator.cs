using FluentValidation;

namespace UserService.Application.AvatarManagement.Commands.CreateAvatarCommand;

public class CreateAvatarCommandValidator : AbstractValidator<CreateAvatarCommand>
{
    public CreateAvatarCommandValidator()
    {
        RuleFor(x => x.AvatarStream).NotEmpty().WithMessage("Avatar must be not empty");
        RuleFor(x => x.UserId).NotEmpty();
    }
}