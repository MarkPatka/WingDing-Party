using UserService.Domain.Common.Abstract;

namespace UserService.Application.IntegrationEvents.Mapping;

public interface IIntegrationEventMapperRegistry
{
    // Return integration event for same domain event
    // or null if not public contract
    IIntegrationEvent? Map(IDomainEvent domainEvent);
}
