using EventService.Application.Persistence;
using EventService.Application.Persistence.Specifications;
using EventService.Application.Services;
using EventService.Domain.EventAggregate.Entities;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Infrastructure.Services;

public class EventTypeService : IEventTypeService
{
    private readonly IRepository<EventType, EventTypeId> _eventTypeRepository;

    public EventTypeService(IRepository<EventType, EventTypeId> eventTypeRepository)
    {
        _eventTypeRepository = eventTypeRepository;
    }

    public async Task<EventTypeId> CreateEventTypeAsync(
        string name, 
        string? description, 
        string? iconUrl = null, 
        CancellationToken cancellationToken = default)
    {
        var eventType = EventType.Create(name, description, iconUrl);
        await _eventTypeRepository.AddAsync(eventType, cancellationToken);
        return eventType.Id;
    }

    public async Task<bool> EventTypeIdExistsAsync(
        EventTypeId id, CancellationToken cancellationToken = default)
    {
        var spec = new EventTypeByIdSpecification(id);
        return await _eventTypeRepository.AnyAsync(spec, cancellationToken);
    }

    public async Task<IReadOnlyList<EventType>> GetAllEventTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _eventTypeRepository.ListAsync(cancellationToken);
    }

    public async Task<EventType?> GetEventTypeByIdAsync(
        EventTypeId id, CancellationToken cancellationToken = default)
    {
        return await _eventTypeRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<EventType?> GetDefaultEventTypeAsync(
        CancellationToken cancellationToken = default)
    {
        var spec = new DefaultEventTypeSpecification();
        return await _eventTypeRepository.FirstOrDefaultAsync(spec, cancellationToken);
    }
}
