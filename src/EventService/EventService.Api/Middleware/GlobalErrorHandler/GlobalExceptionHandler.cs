using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EventService.Application.Common.Errors;

namespace EventService.Api.Middleware.GlobalErrorHandler;

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
        var statusCode = exception switch
        {
            ValidationError => StatusCodes.Status400BadRequest,
            IServiceError serviceError => (int)serviceError.StatusCode,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;

        logger.LogError(
            exception,
            $"Unhandled exception occurred. " +
            $"Path: {httpContext.Request.Path}, " +
            $"Method: {httpContext.Request.Method}, " +
            $"User: {httpContext.User?.Identity?.Name ?? "Anonymous"}");

        var problemDetails = exception switch
        {
            ValidationError validationError => new ProblemDetails
                {
                    Title = "Validation Error",
                    Status = statusCode,
                    Extensions = { { "errors", validationError.Errors } }
                },

            IServiceError serviceError => new ProblemDetails
                {
                    Title = "Service Error",
                    Detail = serviceError.ErrorMessage,
                    Status = statusCode
                },
            
            _ => new ProblemDetails
            {
                Title = "An unexpected error occurred",
                Status = statusCode
            }
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}
