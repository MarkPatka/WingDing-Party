using EventService.Application.Persistence;
using EventService.Application.Persistence.Specifications;
using EventService.Application.Services;
using EventService.Domain;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly IRepository<Event, EventId> _eventRepository;

    public EventService(IRepository<Event, EventId> eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<bool> CheckEventNotExists(
        string title, UserId organizerId, CancellationToken cancellationToken)
    {
        var spec = new GetEventByTitleAndOrganizerSpecification(title, organizerId);
        return await _eventRepository.AnyAsync(spec, cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetEventsByOrganizerIdAsync(
        UserId id, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var spec = new EventsByOrganizerSpec(id, pageNumber, pageSize);
        return await _eventRepository.ListAsync(spec, cancellationToken);
    }

    public async Task<Event?> GetEventByIdAsync(
        EventId id, CancellationToken cancellationToken)
    {
        var spec = new EventByIdSpec(id);
        return await _eventRepository.FirstOrDefaultAsync(spec, cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetActiveEventsAsync(
        CancellationToken cancellationToken)
    {
        var spec = new ActiveEventsSpecification(DateTime.UtcNow);
        return await _eventRepository.ListAsync(spec, cancellationToken);
    }

    public async Task<Event> CreateEventAsync(
        Event @event,
        CancellationToken cancellationToken)
    {
        await _eventRepository.AddAsync(@event, cancellationToken);
        return @event;
    }

    public async Task<Event> UpdateEventAsync(
        Event @event, 
        CancellationToken cancellationToken)
    {
        await _eventRepository.UpdateAsync(@event, cancellationToken);
        return @event;
    }

    public async Task<bool> DeleteEventAsync(
        Event @event, 
        CancellationToken cancellationToken)
    {
        await _eventRepository.DeleteAsync(@event, cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Event>> GetEventsByTextAndFiltersAsync(
        string text, 
        string? eventType, 
        string? city, 
        DateTime? dateFrom, 
        DateTime? dateTo, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var spec = new EventsByTextAndFiltersSpecification(
            text, eventType, city, dateFrom, dateTo, pageNumber, pageSize);
        return await _eventRepository.ListAsync(spec, cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetTopRatedEventsByStartDateWithLimitAsync(
        DateTime startDate, int limit, CancellationToken cancellationToken)
    {
        var spec = new TopRatedEventsByStartDateWithLimitSpecification(startDate, limit);
        return await _eventRepository.ListAsync(spec, cancellationToken);
    }

    public async Task<Event> RegisterParticipantAsync(
        Event @event, 
        UserId userId, 
        string UserName, 
        CancellationToken cancellationToken)
    {
        await _eventRepository.UpdateAsync(@event, cancellationToken);
        return @event;
    }
}
