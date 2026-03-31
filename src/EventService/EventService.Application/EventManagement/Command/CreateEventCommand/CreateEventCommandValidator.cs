using FluentValidation;

namespace EventService.Application.EventManagement.Command.CreateEventCommand;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Title).MaximumLength(100);
        RuleFor(x => x.StartDate).GreaterThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.MaxParticipants).GreaterThan(0);
        RuleFor(x => x.OrganizerId).NotEmpty();
    }
}
