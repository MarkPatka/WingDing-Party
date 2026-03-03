using FluentValidation;

namespace EventService.Application.EventManagement.Queries.GetEventsByTextAndFiltersQuery;

public class GetEventsByTextAndFiltersQueryValidator
    : AbstractValidator<GetEventsByTextAndFiltersQuery>
{
    public GetEventsByTextAndFiltersQueryValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(100);
        RuleFor(x => x.EventType).MaximumLength(100);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.DateFrom).GreaterThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom);
        RuleFor(x => x.PageSize).LessThan(200);
    }
}
