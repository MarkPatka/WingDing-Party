using FluentValidation;

namespace EventService.Application.EventManagement.Command.CreateEventCommand;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.EventTypeId).NotEmpty();
        RuleFor(x => x.StartDate).GreaterThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.MaxParticipants).GreaterThan(1);
        RuleFor(x => x.OrganizerId).NotEmpty();
    }
}
