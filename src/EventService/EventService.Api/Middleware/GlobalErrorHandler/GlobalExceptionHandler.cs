using EventService.Application.Common.Errors;
using EventService.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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
        logger.LogError(
            exception,
            $"Unhandled exception occurred. " +
            $"Path: {httpContext.Request.Path}, " +
            $"Method: {httpContext.Request.Method}, " +
            $"User: {httpContext.User?.Identity?.Name ?? "Anonymous"}");

        var (statusCode, problemDetails) = exception switch
        {
            ValidationError validationError => (
                StatusCodes.Status400BadRequest,
                new ProblemDetails
                {
                    Title = "Validation Error",
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { { "errors", validationError.Errors } }
                }),

            IServiceError serviceError => (
                (int)serviceError.StatusCode,
                new ProblemDetails
                {
                    Title = "Service Error",
                    Detail = serviceError.ErrorMessage,
                    Status = (int)serviceError.StatusCode
                }),

            EntityNotFoundException entityNotFoundException => (
                StatusCodes.Status404NotFound,
                new ProblemDetails
                {
                    Title = "Entity not found",
                    Status = StatusCodes.Status404NotFound,
                    Extensions = { { "reason", entityNotFoundException.Message } }
                }),

            InvalidOperationException invalidOperationException => (
                StatusCodes.Status400BadRequest,
                new ProblemDetails
                {
                    Title = "Invalid Operation Exception",
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { { "reason", invalidOperationException.Message } }
                }),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError
                })
        };

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}
