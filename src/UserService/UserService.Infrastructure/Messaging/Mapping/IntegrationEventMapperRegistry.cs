using Microsoft.Extensions.Logging;
using UserService.Application.IntegrationEvents;
using UserService.Application.IntegrationEvents.Mapping;
using UserService.Domain.Common.Abstract;

namespace UserService.Infrastructure.Messaging.Mapping;

public sealed class IntegrationEventMapperRegistry : IIntegrationEventMapperRegistry
{
    private readonly IReadOnlyDictionary<Type, IIntegrationEventMapper> _mappers;
    private readonly ILogger<IntegrationEventMapperRegistry> _logger;

    public IntegrationEventMapperRegistry(
        IEnumerable<IIntegrationEventMapper> mappers,
        ILogger<IntegrationEventMapperRegistry> logger)
    {
        _logger = logger;

        var dict = new Dictionary<Type, IIntegrationEventMapper>();
        foreach (var mapper in mappers)
        {
            if (!dict.TryAdd(mapper.DomainEventType, mapper))
                throw new InvalidOperationException(
                    $"Duplicate mapper for {mapper.DomainEventType.Name}: " +
                    $"already registered {dict[mapper.DomainEventType].GetType().Name}, " +
                    $"now adding {mapper.GetType().Name}");
        }

        _mappers = dict;

        _logger.LogInformation(
            "IntegrationEventMapperRegistry initialized with {Count} mappers: [{Mappers}]",
            _mappers.Count,
            string.Join(", ", _mappers.Values.Select(m => m.GetType().Name)));
    }

    public IIntegrationEvent? Map(IDomainEvent domainEvent)
    {
        if (!_mappers.TryGetValue(domainEvent.GetType(), out var mapper))
        {
            // Not all domain events translated to integration events
            // For example, event "UserProfileBioUpdated" may stay inside
            return null;
        }

        return mapper.Map(domainEvent);
    }
}
