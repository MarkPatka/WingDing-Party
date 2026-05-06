using UserService.Domain.Common.Abstract;

namespace UserService.Application.IntegrationEvents.Mapping;

public abstract class IntegrationEventMapper<TDomainEvent, TIntegrationEvent>
    : IIntegrationEventMapper
    where TDomainEvent : IDomainEvent
    where TIntegrationEvent : IIntegrationEvent
{
    public Type DomainEventType => typeof(TDomainEvent);

    public IIntegrationEvent Map(IDomainEvent domainEvent)
    {
        if (domainEvent is not TDomainEvent typed)
            throw new InvalidOperationException(
                $"{GetType().Name} cannot map {domainEvent.GetType().Name}. " +
                $"Expected {typeof(TDomainEvent).Name}");

        return MapCore(typed);
    }

    protected abstract IIntegrationEvent MapCore(TDomainEvent typed);
}
