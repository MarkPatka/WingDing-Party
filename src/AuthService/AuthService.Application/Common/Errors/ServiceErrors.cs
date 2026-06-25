using System.Net;

namespace AuthService.Application.Common.Errors;

/// <summary>
/// Catalogue of application-level errors. Each factory returns a throwable
/// <see cref="ServiceError"/> with the status code the API should respond with.
/// </summary>
public static class ServiceErrors
{
    public static ServiceError UserNotFound(Guid userId) =>
        new(HttpStatusCode.NotFound, $"User '{userId}' was not found.");

    public static ServiceError InvalidRole(string role) =>
        new(HttpStatusCode.BadRequest, $"'{role}' is not a valid role.");
}
