using AuthService.Domain.Common.Abstractions;
using AuthService.Domain.Enumerations;
using AuthService.Domain.ValueObjects.Ids;

namespace AuthService.Domain.Entities;

public sealed class User : Entity<UserId>
{
    private readonly List<Role> _roles = [];

    public IReadOnlyCollection<Role> Roles => [.. _roles];

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Keycloak Subject ID (sub claim). Links local user to Keycloak identity.
    /// </summary>
    public string IdentityId { get; private set; } = string.Empty;

    private User() { }
    private User(UserId id, string firstName, string lastName, string email)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public static User Create(string firstName, string lastName, string email)
    {
        var user = new User(UserId.CreateUnique(), firstName, lastName, email);
        user.AddRole(Role.Create(RoleType.Registered));
        return user;
    }

    public void AddRole(Role role)
    {
        _roles.Add(role);
    }

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }


}
