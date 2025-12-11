using System.Net;
using WingDing_Party.AuthenticationService.Application.Common.Errors.Abstract;

namespace WingDing_Party.AuthenticationService.Application.Common.Errors;

public class ValidationError(string message, HttpStatusCode httpStatusCode, IReadOnlyDictionary<string, string[]> errorsDictionary)
    : Error(ErrorType.VALIDATION, message, httpStatusCode)
{
    public IReadOnlyDictionary<string, string[]> Errors => errorsDictionary;

    public List<Exception> Flatten() =>
        [.. Errors.Values.SelectMany(mess => mess.Select(ex => new Exception(ex)))];
}
