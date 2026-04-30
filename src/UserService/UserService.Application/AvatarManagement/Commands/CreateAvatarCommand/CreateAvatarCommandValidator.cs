using FluentValidation;

namespace UserService.Application.AvatarManagement.Commands.CreateAvatarCommand;

public class CreateAvatarCommandValidator : AbstractValidator<CreateAvatarCommand>
{
    public CreateAvatarCommandValidator()
    {
        RuleFor(x => x.AvatarStream).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}