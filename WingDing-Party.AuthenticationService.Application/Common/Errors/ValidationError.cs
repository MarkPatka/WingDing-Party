using System.Net;
using WingDing_Party.AuthenticationService.Application.Common.Errors.Abstract;

namespace WingDing_Party.AuthenticationService.Application.Common.Errors;

public class ValidationError(string message, HttpStatusCode httpStatusCode, IReadOnlyDictionary<string, string[]> errorsDictionary)
    : Error(ErrorType.VALIDATION, message), IServiceException
{
    public IReadOnlyDictionary<string, string[]> Errors => errorsDictionary;

    public HttpStatusCode StatusCode => httpStatusCode;

    public string ErrorMessage => Message;

    public List<Exception> Flatten() =>
        [.. Errors.Values.SelectMany(mess => mess.Select(ex => new Exception(ex)))];
}
