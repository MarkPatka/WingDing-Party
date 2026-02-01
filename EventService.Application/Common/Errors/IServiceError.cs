using System.Net;

namespace EventService.Application.Common.Errors;

public interface IServiceError
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorMessage { get; }
}
