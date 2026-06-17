using AuthService.Application.Common.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Middleware.GlobalErrorHandler;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unhandled exception occurred. Path: {Path}, Method: {Method}, User: {User}",
            httpContext.Request.Path,
            httpContext.Request.Method,
            httpContext.User?.Identity?.Name ?? "Anonymous");

        (int statusCode, string detail) = exception switch
        {
            IServiceError serviceError => (
                (int)serviceError.StatusCode,
                serviceError.ErrorMessage),

            ArgumentException argumentException => (
                StatusCodes.Status400BadRequest,
                argumentException.Message),

            InvalidOperationException invalidOperationException => (
                StatusCodes.Status400BadRequest,
                invalidOperationException.Message),

            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred. Please try again later.")
        };

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "An error occurred",
                Detail = detail,
                Status = statusCode
            }
        });
    }
}
