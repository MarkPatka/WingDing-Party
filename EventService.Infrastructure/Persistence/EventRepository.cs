using EventService.Application.Persistence;
using EventService.Application.Services;
using EventService.Domain;
using System.Linq.Expressions;

namespace EventService.Infrastructure.Persistence;

public class EventRepository : IEventRepository
{
    private readonly ITimeProviderService _timeProviderService;

    public EventRepository(ITimeProviderService timeProviderService)
    {
        _timeProviderService = timeProviderService;
    }

    public Task Add(Event entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Event entity)
    {
        throw new NotImplementedException();
    }

    public Task<Event> Get(Expression<Func<Event, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Event>> GetByFilter(Expression<Func<Event, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task Update(Event entity)
    {
        throw new NotImplementedException();
    }
}
