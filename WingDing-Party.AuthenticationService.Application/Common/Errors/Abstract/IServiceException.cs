using System.Net;

namespace WingDing_Party.AuthenticationService.Application.Common.Errors.Abstract;

public interface IServiceException 
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorMessage { get; }
}
