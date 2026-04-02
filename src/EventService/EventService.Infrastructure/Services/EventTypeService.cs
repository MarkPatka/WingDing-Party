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

    public async Task<EventType> CreateEventTypeAsync(
        EventType eventType,
        CancellationToken cancellationToken = default)
    {
        await _eventTypeRepository.AddAsync(eventType, cancellationToken);
        return eventType;
    }

    public async Task<bool> EventTypeIdExistsAsync(
        EventTypeId id, CancellationToken cancellationToken = default)
    {
        var spec = new EventTypeByIdSpecification(id);
        return await _eventTypeRepository.AnyAsync(spec, cancellationToken);
    }

    public async Task<IReadOnlyList<EventType>> GetAllEventTypesAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var spec = new EventTypesSpecification(pageNumber, pageSize);
        return await _eventTypeRepository.ListAsync(spec, cancellationToken);
    }

    public async Task<EventType?> GetEventTypeByIdAsync(
        EventTypeId id, CancellationToken cancellationToken = default)
    {
        var spec = new EventTypeByIdSpecification(id);
        return await _eventTypeRepository.FirstOrDefaultAsync(spec, cancellationToken);
    }

    public async Task<EventType?> GetDefaultEventTypeAsync(
        CancellationToken cancellationToken = default)
    {
        var spec = new DefaultEventTypeSpecification();
        return await _eventTypeRepository.FirstOrDefaultAsync(spec, cancellationToken);
    }

    public async Task<bool> CheckEventTypeNotExists(
        string name, CancellationToken cancellationToken = default)
    {
        var spec = new EventTypeByNameSpecification(name);
        return await _eventTypeRepository.AnyAsync(spec, cancellationToken);
    }
}
