using UserService.Application.IntegrationEvents;

namespace UserService.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage() { }

    public OutboxMessage(IIntegrationEvent integrationEvent, string payload, string type)
    {
        Id = integrationEvent.Id;
        OccurredOnUtc = integrationEvent.OccurredOnUtc;
        Type = type;
        Payload = payload;
    }

    public Guid Id { get; private set; }

    public DateTime OccurredOnUtc { get; private set; }

    public string Type { get; private set; } = default!;

    public string Payload { get; private set; } = default!;

    public DateTime? ProcessedOnUtc { get; private set; }

    public string? Error { get; private set; }

    public int Retries { get; private set; }

    public void MarkAsProcessed()
    {
        ProcessedOnUtc = DateTime.UtcNow;
    }

    public void MarkAsFailed(string error)
    {
        Retries++;
        Error = error;
    }
}