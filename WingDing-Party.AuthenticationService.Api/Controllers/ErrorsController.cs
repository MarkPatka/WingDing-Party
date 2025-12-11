using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using WingDing_Party.AuthenticationService.Application.Common.Errors;
using WingDing_Party.AuthenticationService.Application.Common.Errors.Abstract;

namespace WingDing_Party.AuthenticationService.Api.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ApiController
{
    private readonly ILogger<ErrorsController> _logger;
    private readonly IHostEnvironment _environment;

    public ErrorsController(
        ILogger<ErrorsController> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    [Route("/error")]
    public IActionResult Error()
    {
        var exceptionHandlerFeature = HttpContext.Features
            .Get<IExceptionHandlerFeature>();
        
        var exception = exceptionHandlerFeature?.Error;

        if (exception == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unknown error occurred");
        }

        // Log the exception with context
        _logger.LogError(
            exception,
            "Unhandled exception occurred. Path: {Path}, Method: {Method}, User: {User}",
            HttpContext.Request.Path,
            HttpContext.Request.Method,
            HttpContext.User?.Identity?.Name ?? "Anonymous");

        // Handle specific exception types differently
        var (statusCode, message) = exception switch
        {
            // ValidationError validationError => ((int)validationError.StatusCode, validationError.ErrorMessage),
            IServiceException serviceException => ((int)serviceException.StatusCode, serviceException.ErrorMessage),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occured")
        };

        return Problem(statusCode: statusCode, title: message);
    }

    [Route("/error-development")]
    public IActionResult ErrorDevelopment()
    {
        if (!_environment.IsDevelopment())
            return NotFound();

        var exceptionHandlerFeature = HttpContext.Features
            .Get<IExceptionHandlerFeature>();

        if (exceptionHandlerFeature?.Error is not Error error)
            return Problem();

        return Problem(error); 
    }
}
