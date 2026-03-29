using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventService.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest>? _validator;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        ILogger<ValidationBehavior<TRequest, TResponse>> logger,
        IValidator<TRequest>? validator = null)
    {
        _logger = logger;
        _validator = validator;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator is null)
            return await next(cancellationToken);

        var validationResult = await _validator
            .ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var errors = _validator.Validate(context).Errors
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToArray()
                })
            .ToDictionary(x => x.Key, x => x.Values);

        var errorDetails = string.Join("; ",
            errors.Select(kvp => $"[{kvp.Key}: {string.Join(", ", kvp.Value)}]"));

        _logger.LogWarning("Validation failed for {RequestType}: {Errors}",
            typeof(TRequest).Name, errorDetails);

        if (errors.Count != 0)
        {
            throw new Errors.ValidationError(
                $"Validation failed: {errorDetails}",
                System.Net.HttpStatusCode.BadRequest,
                errors);
        }
        return await next(cancellationToken);
    }
}
