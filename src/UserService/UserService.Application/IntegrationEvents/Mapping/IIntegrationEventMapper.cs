using UserService.Domain.Common.Abstract;

namespace UserService.Application.IntegrationEvents.Mapping;

public interface IIntegrationEventMapper
{
    Type DomainEventType { get; }
    IIntegrationEvent Map(IDomainEvent domainEvent);
}
