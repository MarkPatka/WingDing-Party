using FluentValidation;

namespace UserService.Application.UserProfileManagement.Queries.GetUserProfileQuery;

public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}