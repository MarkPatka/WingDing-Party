using EventService.Application.Common.Configuration;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure.EventSourcing.Outbox;

internal sealed class OutboxOptionsValidator : IValidateOptions<OutboxOptions>
{
    public ValidateOptionsResult Validate(string? name, OutboxOptions options)
    {
        var errors = new List<string>();

        if (options.BatchSize < 1 || options.BatchSize > 1000)
            errors.Add($"BatchSize must be between 1 and 1000, was {options.BatchSize}");

        if (options.PollingIntervalMs < 1 || options.PollingIntervalMs > 600_000)
            errors.Add($"PollingIntervalMs must be between 1 and 600000 (10 min), was {options.PollingIntervalMs}");

        if (options.MaxRetryAttempts < 0 || options.MaxRetryAttempts > 100)
            errors.Add($"MaxRetryAttempts must be between 0 and 100, was {options.MaxRetryAttempts}");

        return errors.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(errors);
    }
}