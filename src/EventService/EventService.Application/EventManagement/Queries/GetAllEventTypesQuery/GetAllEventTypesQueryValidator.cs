using FluentValidation;

namespace EventService.Application.EventManagement.Queries.GetAllEventTypesQuery;

public class GetAllEventTypesQueryValidator : AbstractValidator<GetAllEventTypesQuery>
{
	public GetAllEventTypesQueryValidator()
	{
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}
