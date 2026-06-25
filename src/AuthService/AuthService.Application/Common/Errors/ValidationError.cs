using FluentValidation;
using FluentValidation.Results;
using System.Net;

namespace AuthService.Application.Common.Errors;

public sealed class ValidationError(string message, IEnumerable<ValidationFailure> errors)
    : ValidationException(message, errors), IServiceError
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public string ErrorMessage => Message;
}
