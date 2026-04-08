using System.Net;

namespace EventService.Application.Common.Errors;

public record ServiceError
(
    HttpStatusCode StatusCode,
    string ErrorMessage
) : IServiceError
{
    public HttpStatusCode StatusCode { get; } = StatusCode;
    public string ErrorMessage { get; } = ErrorMessage;
}
