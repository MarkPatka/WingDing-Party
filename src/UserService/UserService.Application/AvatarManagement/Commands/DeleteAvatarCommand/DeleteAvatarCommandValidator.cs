using FluentValidation;

namespace UserService.Application.AvatarManagement.Commands.DeleteAvatarCommand;

public class DeleteAvatarCommandValidator : AbstractValidator<DeleteAvatarCommand>
{
    public DeleteAvatarCommandValidator()
    {
        RuleFor(x => x.AvatarId).NotEmpty();
    }
}