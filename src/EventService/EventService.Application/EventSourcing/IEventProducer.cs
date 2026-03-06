namespace EventService.Application.EventSourcing;

public interface IEventProducer : IDisposable
{
    Task ProduceAsync<TMessage>(
        string topic,
        string key,
        TMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : class;
}
