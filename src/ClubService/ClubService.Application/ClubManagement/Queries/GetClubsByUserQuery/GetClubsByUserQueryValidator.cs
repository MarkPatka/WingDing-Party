using FluentValidation;

namespace ClubService.Application.ClubManagement.Queries.GetClubsByUserQuery;

public class GetClubsByUserQueryValidator : AbstractValidator<GetClubsByUserQuery>
{
    public GetClubsByUserQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}