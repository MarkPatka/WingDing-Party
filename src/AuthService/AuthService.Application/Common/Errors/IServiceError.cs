using System.Net;

namespace AuthService.Application.Common.Errors;

public interface IServiceError
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorMessage { get; }
}
