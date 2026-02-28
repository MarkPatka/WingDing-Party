namespace EventService.Application.EventSourcing;

public abstract record IntergrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccuredOn { get; init; } = DateTime.UtcNow;
    public string EventType { get; init; } = string.Empty;
}
