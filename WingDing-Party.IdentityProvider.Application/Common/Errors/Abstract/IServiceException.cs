using System.Net;

namespace WingDing_Party.IdentityProvider.Application.Common.Errors.Abstract;

public interface IServiceException 
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorMessage { get; }
}
