using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace WingDing_Party.IdentityProvider.Api.Common.Errors;

public class IdentityProviderProblemDetailsFactory 
    : ProblemDetailsFactory
{
    private readonly ApiBehaviorOptions _options;
    private readonly IHostEnvironment? _environment;

    public IdentityProviderProblemDetailsFactory(
        IOptions<ApiBehaviorOptions> options,
        IHostEnvironment? environment = null)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _environment = environment;
    }

    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext, 
        int? statusCode = null, 
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        statusCode ??= 500;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance
        };

        ApplyProblemDetailsDefaults(
            httpContext, problemDetails, statusCode.Value);
    
        return problemDetails;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext, 
        ModelStateDictionary modelStateDictionary, 
        int? statusCode = null, 
        string? title = null, 
        string? type = null, 
        string? detail = null, 
        string? instance = null)
    {
        ArgumentNullException.ThrowIfNull(modelStateDictionary);

        statusCode ??= 400;

        var problemDetails = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode,
            Type = type,
            Detail = detail,
            Instance = instance
        };

        // Set title if provided, otherwise it will use the default
        if (title != null)
        {
            problemDetails.Title = title;
        }

        ApplyProblemDetailsDefaults(
            httpContext, problemDetails, statusCode.Value);

        return problemDetails;
    }

    private void ApplyProblemDetailsDefaults(
        HttpContext httpContext, 
        ProblemDetails problemDetails, 
        int statusCode)
    {
        problemDetails.Status ??= statusCode;

        if (_options.ClientErrorMapping.TryGetValue(statusCode, out var clientErrorData))
        {
            problemDetails.Title ??= clientErrorData.Title;
            problemDetails.Type ??= clientErrorData.Link;
        }

        var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }
        // Add timestamp for when the error occurred
        problemDetails.Extensions["timestamp"] = TimeProvider.System.GetUtcNow().ToString("o");

        // Add request path
        if (httpContext != null)
        {
            problemDetails.Instance ??= httpContext.Request.Path;

            // Add request method
            problemDetails.Extensions["method"] = httpContext.Request.Method;

            // Add user information if authenticated
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                var userId = httpContext.User.FindFirst("sub")?.Value
                    ?? httpContext.User.FindFirst("id")?.Value
                    ?? httpContext.User.Identity.Name;

                if (userId != null)
                {
                    problemDetails.Extensions["userId"] = userId;
                }
            }

            // Add correlation ID if exists (useful for distributed tracing)
            if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
            {
                problemDetails.Extensions["correlationId"] = correlationId.ToString();
            }
        }

        // Add environment info only in development (security consideration)
        if (_environment?.IsDevelopment() == true)
        {
            problemDetails.Extensions["environment"] = _environment.EnvironmentName;
        }

        // Add helpful links for common status codes
        // They're meant to:
        //     Help developers understand why they got that error
        //     Provide documentation on how to fix the issue
        //     Explain what parameters are required, authentication flows, etc.

        if (!problemDetails.Extensions.ContainsKey("helpLink"))
        {
            problemDetails.Extensions["helpLink"] = statusCode switch
            {
                400 => "https://docs.yourapp.com/errors/bad-request",
                401 => "https://docs.yourapp.com/errors/unauthorized",
                403 => "https://docs.yourapp.com/errors/forbidden",
                404 => "https://docs.yourapp.com/errors/not-found",
                409 => "https://docs.yourapp.com/errors/conflict",
                422 => "https://docs.yourapp.com/errors/validation",
                500 => "https://docs.yourapp.com/errors/server-error",
                _ => "https://docs.yourapp.com/errors"
            };
        }

    }

}
