namespace AuthService.Contracts.Authentication;

public sealed record AssignRoleRequest(Guid Id, string Role);
public sealed record AssignRoleResponse(string[] Roles);

