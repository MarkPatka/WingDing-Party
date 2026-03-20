using FluentValidation;

namespace UserService.Application.UserProfileManagement.Queries.GetUserProfileInterestsQuery;

public class GetUserProfileInterestsQueryValidator : AbstractValidator<GetUserProfileInterestsQuery>
{
    public GetUserProfileInterestsQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}