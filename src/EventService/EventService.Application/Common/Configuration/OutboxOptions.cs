namespace EventService.Application.Common.Configuration;

public sealed class OutboxOptions
{
    public const string SectionName = nameof(OutboxOptions);
    public int BatchSize { get; init; } = 50;
    public int PollingIntervalMs { get; set; } = 5000;
    public int MaxRetryAttempts { get; set; } = 5;
}
