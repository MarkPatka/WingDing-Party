using FluentValidation;

namespace EventService.Application.EventManagement.Command.RegisterParticipant;

public class RegisterParticipantCommandValidator : AbstractValidator<RegisterParticipantCommand>
{
    public RegisterParticipantCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty();
    }
}
