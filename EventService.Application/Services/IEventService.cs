using EventService.Domain;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Application.Services;

public interface IEventService
{
    public Task<IReadOnlyList<Event>> GetEventsByOrganizerIdAsync(UserId id, int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<bool> CheckEventNotExists(string title, UserId request, CancellationToken cancellationToken);
    public Task<Event?> GetEventByIdAsync(EventId id, CancellationToken cancellationToken = default);
    public Task<IReadOnlyList<Event>> GetActiveEventsAsync(CancellationToken cancellationToken = default);
    public Task<Event> CreateEventAsync(Event @event, CancellationToken cancellationToken);
}
