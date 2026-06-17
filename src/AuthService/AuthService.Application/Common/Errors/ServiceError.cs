using System.Net;

namespace AuthService.Application.Common.Errors;

/// <summary>
/// Throwable application error carrying the HTTP status code to surface.
/// Caught by GlobalExceptionHandler (matches on <see cref="IServiceError"/>) and
/// rendered as ProblemDetails. Build instances via <see cref="ServiceErrors"/>.
/// </summary>
public sealed class ServiceError : Exception, IServiceError
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorMessage => Message;

    public ServiceError(HttpStatusCode statusCode, string message) : base(message)
        => StatusCode = statusCode;
}
