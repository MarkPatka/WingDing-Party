using FluentValidation;

namespace EventService.Application.EventManagement.Command.CreateEventCommand;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.EventType).IsInEnum();
        RuleFor(x => x.StartDate).GreaterThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.MaxParticipants).InclusiveBetween(2, 100);
        RuleFor(x => x.OrganizerId).NotEmpty();
    }
}
