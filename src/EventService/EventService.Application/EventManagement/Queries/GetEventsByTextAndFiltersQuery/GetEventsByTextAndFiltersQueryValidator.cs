using FluentValidation;

namespace EventService.Application.EventManagement.Queries.GetEventsByTextAndFiltersQuery;

public class GetEventsByTextAndFiltersQueryValidator
    : AbstractValidator<GetEventsByTextAndFiltersQuery>
{
    public GetEventsByTextAndFiltersQueryValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(100);

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        RuleFor(x => x.DateFrom)
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.DateFrom.HasValue);

        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom)
            .When(x => x.DateTo.HasValue);

        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
