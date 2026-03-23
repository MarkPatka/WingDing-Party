using FluentValidation;

namespace EventService.Application.EventManagement.Queries.GetTopRatedEventsByStartDateWithLimitQuery;

public class GetTopRatedEventsByStartDateWithLimitQueryValidator 
    : AbstractValidator<GetTopRatedEventsByStartDateWithLimitQuery>
{
    public GetTopRatedEventsByStartDateWithLimitQueryValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();
    }
}
