using AuthService.Domain.Common.Abstractions;

namespace AuthService.Domain.Enumerations;

public sealed class Permission(int id, string name, string? description = null) 
    : Enumeration(id, name, description)
{
    // Event Service Permissions
    public static readonly Permission EventsRead   = new(1, "events:read");
    public static readonly Permission EventsCreate = new(2, "events:create");
    public static readonly Permission EventsUpdate = new(3, "events:update");
    public static readonly Permission EventsDelete = new(4, "events:delete");

    // User Service Permissions
    public static readonly Permission UsersRead    = new(5, "users:read");
    public static readonly Permission UsersUpdate  = new(6, "users:update");

    // Club Service Permissions 
    public static readonly Permission ClubsRead    = new(7, "clubs:read");
    public static readonly Permission ClubsCreate  = new(8, "clubs:create");
    public static readonly Permission ClubsUpdate  = new(9, "clubs:update");
    public static readonly Permission ClubsDelete  = new(10, "clubs:delete");

    // Admin Permissions
    public static readonly Permission AdminPanel   = new(11, "admin:panel");
}
