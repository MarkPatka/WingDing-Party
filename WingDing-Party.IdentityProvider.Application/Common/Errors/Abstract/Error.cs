using System.Net;

namespace WingDing_Party.IdentityProvider.Application.Common.Errors.Abstract;

public abstract class Error(ErrorType type, string message, HttpStatusCode statusCode)
    : Exception(message), IServiceException
{
    public HttpStatusCode StatusCode => statusCode;
    public ErrorType Type => type;
    public string ErrorMessage => Message;
}
