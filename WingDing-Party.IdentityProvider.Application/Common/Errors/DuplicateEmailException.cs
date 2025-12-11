using System.Net;
using WingDing_Party.IdentityProvider.Application.Common.Errors.Abstract;

namespace WingDing_Party.IdentityProvider.Application.Common.Errors;

public class DuplicateEmailException(string message, HttpStatusCode httpStatusCode)
    : Error(ErrorType.VALIDATION, message, httpStatusCode)
{
}
