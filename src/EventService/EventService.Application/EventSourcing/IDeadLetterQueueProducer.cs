namespace EventService.Application.EventSourcing;

public interface IDeadLetterQueueProducer : IDisposable
{
    public Task PublishAsync(
        string originalTopic,
        string? messageKey,
        string? messageValue,
        string errorReason,
        Exception? exception = null,
        CancellationToken ct = default);
}
