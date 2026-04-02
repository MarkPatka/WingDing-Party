using EventService.Domain.EventAggregate.Entities;

namespace EventService.Application.Persistence.Specifications;

public class EventTypesSpecification : BaseSpecification<EventType>
{
    public EventTypesSpecification(int pageNumber, int pageSize)
    {
        ApplyPaging(
            skip: (pageNumber - 1) * pageSize,
            take: pageSize);

        ApplyNoTracking();
    }
}
