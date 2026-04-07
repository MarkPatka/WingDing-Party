using FluentValidation;
using System.Net;

namespace EventService.Application.Common.Errors;

public class ValidationError : ValidationException, IServiceError
{
    private readonly HttpStatusCode _httpStatusCode;
    private readonly IReadOnlyDictionary<string, string[]> _errorsDictionary;

    public ValidationError(
        string message,
        HttpStatusCode httpStatusCode,
        IReadOnlyDictionary<string, string[]> errorsDictionary)
        : base(message)
    {
        _httpStatusCode = httpStatusCode;
        _errorsDictionary = errorsDictionary;
    }
    public new IReadOnlyDictionary<string, string[]> Errors => _errorsDictionary;

    public HttpStatusCode StatusCode => _httpStatusCode;

    public string ErrorMessage => Message;

    public List<Exception> Flatten() =>
        [.. Errors.Values.SelectMany(mess => mess.Select(ex => new Exception(ex)))];
}

