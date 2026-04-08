using FluentValidation;

namespace EventService.Application.EventManagement.Command.CreateEventTypeCommand;

public class CreateEventTypeCommandValidator : AbstractValidator<CreateEventTypeCommand>
{
    public CreateEventTypeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
