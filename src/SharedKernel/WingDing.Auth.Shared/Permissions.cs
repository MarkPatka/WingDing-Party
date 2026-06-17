namespace WingDing.Auth.Shared;

/// <summary>
/// String constants matching Permission.Name values from the domain.
/// Used in [HasPermission(Permissions.EventsCreate)] attributes.
/// </summary>
public static class Permissions
{
    public const string EventsRead   = "events:read";
    public const string EventsCreate = "events:create";
    public const string EventsUpdate = "events:update";
    public const string EventsDelete = "events:delete";
    public const string UsersRead    = "users:read";
    public const string UsersUpdate  = "users:update";
    public const string ClubsRead    = "clubs:read";
    public const string ClubsCreate  = "clubs:create";
    public const string ClubsUpdate  = "clubs:update";
    public const string ClubsDelete  = "clubs:delete";
    public const string AdminPanel   = "admin:panel";
}