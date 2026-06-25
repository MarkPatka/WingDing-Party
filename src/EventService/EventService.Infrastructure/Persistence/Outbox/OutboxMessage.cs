namespace EventService.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime OccuredOnUtc { get; init; }
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; }
    public OutboxMessage() { }
    public OutboxMessage(
        Guid id,
        string type,
        string content,
        DateTime occuredOnUtc)
    {
        Id = id;
        Type = type;
        Content = content;
        OccuredOnUtc = occuredOnUtc;
    }
}
