using WingDing_Party.IdentityProvider.Domain.Common.Abstract;
using WingDing_Party.IdentityProvider.Domain.Common.ResultPattern;

namespace WingDing_Party.IdentityProvider.Domain.UserAggregate.ValueObjects;

public sealed class Permission : ValueObject
{
    public string Resource { get; } // "catalog", "order", "identity"
    public string Scope { get; }    // "product", "user", "role"
    public string Action { get; }   // "read", "write", "delete"

    private Permission(string resource, string scope, string action)
    {
        Resource = resource;
        Scope = scope;
        Action = action;
    }

    public static Result<Permission> Create(string permissionString)
    {
        var parts = permissionString.Split(':');

        if (parts.Length != 3)
        {
            return Result<Permission>.Failure(
                "Invalid permission format. Expected: resource:scope:action");
        }

        return Result<Permission>.Success(
            new Permission(parts[0], parts[1], parts[2]));
    }

    public string ToPermissionString() => 
        $"{Resource}:{Scope}:{Action}";

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Resource;
        yield return Scope;
        yield return Action;
    }
}
