using FluentValidation;
using MediatR;
using WingDing_Party.IdentityProvider.Application.Authentication.Commands.Register;

namespace WingDing_Party.IdentityProvider.Application.Common.Behaviours;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehavior(IValidator<TRequest>? validator = null)
    {
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

        if (errors.Count != 0)
        {
            throw new Errors.ValidationError(
                $"One or more errors occured while {nameof(RegisterCommand)} validation",
                System.Net.HttpStatusCode.Conflict,
                errors);
        }
        return await next(cancellationToken);
    }
}