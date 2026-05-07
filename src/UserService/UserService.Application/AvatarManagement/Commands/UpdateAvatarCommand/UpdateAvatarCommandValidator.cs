using FluentValidation;

namespace UserService.Application.AvatarManagement.Commands.UpdateAvatarCommand;

public class UpdateAvatarCommandValidator : AbstractValidator<UpdateAvatarCommand>
{
    public UpdateAvatarCommandValidator()
    {
        RuleFor(x => x.AvatarId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x)
            .Must(x => !(x.IsDefault && !x.IsActive))
            .WithMessage("Default avatar must be active.");
    }
}