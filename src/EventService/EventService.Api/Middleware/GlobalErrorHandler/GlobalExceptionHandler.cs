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
        var errorFeature = httpContext.Features
            .Get<IExceptionHandlerFeature>();

        var originalError = errorFeature?.Error;

        if (originalError is null)
        {
            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails
                {
                    Title = "An unknown error occurred",
                    Status = StatusCodes.Status500InternalServerError
                }
            });
        }

        logger.LogError(
            exception,
            $"Unhandled exception occurred. " +
            $"Path: {httpContext.Request.Path}, " +
            $"Method: {httpContext.Request.Method}, " +
            $"User: {httpContext.User?.Identity?.Name ?? "Anonymous"}");

        var (statusCode, message) = exception switch
        {
            IServiceError serviceException => ((int)serviceException.StatusCode, serviceException.ErrorMessage),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = originalError.GetType().Name,
                Title = "An error occurred",
                Detail = message,
                Status = statusCode
            }
        });
    }
}
