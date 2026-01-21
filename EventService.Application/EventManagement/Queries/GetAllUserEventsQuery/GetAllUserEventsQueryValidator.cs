using FluentValidation;

namespace EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;

public class GetAllUserEventsQueryValidator : AbstractValidator<GetAllUserEventsQuery>
{
    public GetAllUserEventsQueryValidator()
    {
        RuleFor(x => x.userId).NotEmpty();
    }
}
