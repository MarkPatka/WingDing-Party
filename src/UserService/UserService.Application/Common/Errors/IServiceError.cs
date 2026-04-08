using System.Net;

namespace UserService.Application.Common.Errors;

public interface IServiceError
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorMessage { get; }
    public string StackTrace { get; }
}
