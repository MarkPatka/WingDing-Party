using FluentValidation;

namespace ClubService.Application.ClubManagement.Queries.GetClubQuery;

public class GetClubQueryValidator : AbstractValidator<GetClubQuery>
{
    public GetClubQueryValidator()
    {
        RuleFor(x => x.ClubId).NotEmpty();
    }
}