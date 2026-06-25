using AuthService.Domain.Common.Abstractions;
using AuthService.Domain.Enumerations;
using FluentValidation;

namespace AuthService.Application.UserManagement.Command.AssignRole;

public sealed class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(BeAValidRole)
            .WithMessage("'{PropertyValue}' is not a valid role.");
    }

    private static bool BeAValidRole(string role) =>
        Enumeration.GetAll<RoleType>().Any(r => r.Name == role);
}
