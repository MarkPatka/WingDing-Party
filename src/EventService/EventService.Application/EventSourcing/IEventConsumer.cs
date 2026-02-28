namespace EventService.Application.EventSourcing;

public interface IEventConsumer : IDisposable
{
    public void Initialize();
    public Task ConsumeMessageAsync(CancellationToken stoppingToken);
    public void Close();
}
