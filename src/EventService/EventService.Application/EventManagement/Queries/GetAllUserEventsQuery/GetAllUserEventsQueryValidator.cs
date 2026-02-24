using FluentValidation;

namespace EventService.Application.EventManagement.Queries.GetAllUserEventsQuery;

public class GetAllUserEventsQueryValidator : AbstractValidator<GetAllUserEventsQuery>
{
    public GetAllUserEventsQueryValidator()
    {
        RuleFor(x => x.OrganizerId).NotEmpty();
        RuleFor(x => x.PageSize).LessThan(200);
    }
}
