using EventService.Application.Common.Configuration;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure.EventSourcing.Messaging;

internal sealed class KafkaOptionsValidator : IValidateOptions<KafkaOptions>
{
    private static readonly HashSet<string> AllowedOffsetReset = new(StringComparer.OrdinalIgnoreCase)
    {
        "Earliest", "Latest", "None"
    };

    public ValidateOptionsResult Validate(string? name, KafkaOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.BootstrapServers))
            errors.Add("BootstrapServers is required");

        if (string.IsNullOrWhiteSpace(options.ConsumerGroupId))
            errors.Add("ConsumerGroupId is required");

        if (string.IsNullOrWhiteSpace(options.ProduceEventsTopic))
            errors.Add("ProduceEventsTopic is required");

        if (options.ConsumeEventsTopics is null || options.ConsumeEventsTopics.Length == 0)
            errors.Add("ConsumeEventsTopics must contain at least one topic");
        else
        {
            if (options.ConsumeEventsTopics.Any(string.IsNullOrWhiteSpace))
                errors.Add("ConsumeEventsTopics must not contain empty/whitespace values");

            var duplicates = options.ConsumeEventsTopics
                .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToArray();
            if (duplicates.Length > 0)
                errors.Add($"ConsumeEventsTopics contains duplicates: {string.Join(", ", duplicates)}");
        }

        if (string.IsNullOrWhiteSpace(options.AutoOffsetReset))
            errors.Add("AutoOffsetReset is required");
        else if (!AllowedOffsetReset.Contains(options.AutoOffsetReset))
            errors.Add(
                $"AutoOffsetReset '{options.AutoOffsetReset}' is invalid. " +
                $"Allowed: {string.Join(", ", AllowedOffsetReset)}");

        if (string.IsNullOrWhiteSpace(options.DeadLetterTopic))
            errors.Add("DeadLetterTopic is required");

        if (options.MaxRetryAttempts < 0)
            errors.Add("MaxRetryAttempts must be >= 0");

        if (options.RetryDelayMs <= 0)
            errors.Add("RetryDelayMs must be > 0");

        return errors.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(errors);
    }
}
