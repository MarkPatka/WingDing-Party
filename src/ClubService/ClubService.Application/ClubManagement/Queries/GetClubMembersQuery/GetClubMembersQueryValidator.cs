using FluentValidation;

namespace ClubService.Application.ClubManagement.Queries.GetClubMembersQuery;

public class GetClubMembersQueryValidator : AbstractValidator<GetClubMembersQuery>
{
    public GetClubMembersQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}