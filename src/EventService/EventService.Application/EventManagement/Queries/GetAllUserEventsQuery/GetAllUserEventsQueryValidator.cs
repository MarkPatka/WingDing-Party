using FluentValidation;

namespace EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;

public class GetAllUserEventsQueryValidator : AbstractValidator<GetAllUserEventsQuery>
{
    public GetAllUserEventsQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}
