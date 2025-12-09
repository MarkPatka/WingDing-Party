using System.Net;
using WingDing_Party.AuthenticationService.Application.Common.Errors.Abstract;

namespace WingDing_Party.AuthenticationService.Application.Common.Errors;

public class DuplicateEmailException(string message, HttpStatusCode httpStatusCode)
    : Error(ErrorType.VALIDATION, message), IServiceException
{
    public HttpStatusCode StatusCode => httpStatusCode;
    public string ErrorMessage => Message;
}
