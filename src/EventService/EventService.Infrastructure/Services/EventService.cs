using EventService.Application.Persistence;
using EventService.Application.Persistence.Specifications;
using EventService.Application.Services;
using EventService.Domain;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly IRepository<Event, EventId> _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EventService(
        IRepository<Event, EventId> eventRepository,
        IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> CheckEventNotExists(
        string title, OrganizerId organizerId, CancellationToken cancellationToken)
    {
        var spec = new GetEventByTitleAndOrganizerSpecification(title, organizerId);
        return await _eventRepository.AnyAsync(spec, cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetEventsByOrganizerIdAsync(
        OrganizerId id, int pageNumber, int pageSize, CancellationToken cancellationToken)
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return @event;
    }
}
