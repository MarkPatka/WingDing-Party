using FluentValidation;

namespace EventService.Application.EventManagement.Command.DeleteEventCommand;

public class DeleteEventCommandValidator : AbstractValidator<DeleteEventCommand>
{
    public DeleteEventCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
    }
}
