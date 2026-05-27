namespace UserService.Application.IntegrationEvents.Mapping;

public interface IIntegrationEventTypeResolver
{
    Type? Resolve(Type domainEventType);
}
