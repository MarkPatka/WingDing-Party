using FluentValidation;

namespace ClubService.Application.ClubManagement.Queries.SearchClubsQuery;

public class SearchClubsQueryValidator : AbstractValidator<SearchClubsQuery>
{
    public SearchClubsQueryValidator()
    {
        RuleFor(x => x).Must(x => !string.IsNullOrEmpty(x.Name) || x.Interests?.Count() > 0);
    }
}