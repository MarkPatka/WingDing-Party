using FluentValidation;

namespace EventService.Application.EventManagement.Command.UpdateEventCommand;

public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.StartDate.HasValue);

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
