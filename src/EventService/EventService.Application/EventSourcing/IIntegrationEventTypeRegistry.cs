namespace EventService.Application.EventSourcing;

public interface IIntegrationEventTypeRegistry
{
    bool TryResolve(string eventType, out Type? type);
    string GetName<TEvent>() where TEvent : IntegrationEvent;
}
