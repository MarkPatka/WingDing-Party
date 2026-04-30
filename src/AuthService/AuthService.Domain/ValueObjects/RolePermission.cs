using AuthService.Domain.Common.Abstractions;

namespace AuthService.Domain.ValueObjects;

public sealed class RolePermission : ValueObject
{
    public int RoleId { get; }
    public int PermissionId { get; }

    private RolePermission(int roleId, int permissionId)
        => (RoleId, PermissionId) = (roleId, permissionId);

    public static RolePermission Create(int roleId, int permissionId) =>
        new(roleId, permissionId);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return RoleId;
        yield return PermissionId;
    }
}
