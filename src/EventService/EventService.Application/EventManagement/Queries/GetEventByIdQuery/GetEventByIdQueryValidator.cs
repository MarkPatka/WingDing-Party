using FluentValidation;

namespace EventService.Application.EventManagement.Queries.GetEventByIdQuery;

public class GetEventByIdQueryValidator : AbstractValidator<GetEventByIdQuery>
{
    public GetEventByIdQueryValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
    }
}
