using System.Net;

namespace ClubService.Application.Common.Errors;

public class ValidationError(
    string message, 
    HttpStatusCode httpStatusCode, 
    IReadOnlyDictionary<string, string[]> errorsDictionary)
    : Exception, IServiceError
{
    public IReadOnlyDictionary<string, string[]> Errors => errorsDictionary;

    public HttpStatusCode StatusCode => httpStatusCode;

    public string ErrorMessage => message;

    public List<Exception> Flatten() =>
        [.. Errors.Values.SelectMany(mess => mess.Select(ex => new Exception(ex)))];
}
