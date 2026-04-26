using ClubService.Application.Common.Errors;
using ClubService.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.Api.Middleware.GlobalErrorHandler;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger,
    IWebHostEnvironment environment)
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
            "Unhandled exception occurred. " +
            $"Path: {httpContext.Request.Path}, " +
            $"Method: {httpContext.Request.Method}, " +
            $"User: {httpContext.User?.Identity?.Name ?? "Anonymous"}");

        var (statusCode, message, stackTrace) = exception switch
        {
            IServiceError serviceException => ((int)serviceException.StatusCode, serviceException.ErrorMessage,
                serviceException.StackTrace),

            EntityNotFoundException => (StatusCodes.Status404NotFound,
                exception.Message,
                string.Empty),

            AlreadyDoneException => (StatusCodes.Status502BadGateway,
                exception.Message,
                string.Empty),

            _ => (StatusCodes.Status500InternalServerError,
                environment.IsDevelopment() ? exception.Message : "an error occured",
                environment.IsDevelopment() ? exception.StackTrace : string.Empty)
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = originalError.GetType().Name,
                Title = message,
                Detail = stackTrace,
                Status = statusCode,
            }
        });
    }
}