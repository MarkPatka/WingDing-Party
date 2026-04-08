using EventService.Domain;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Application.Persistence.Specifications;

public class EventsByOrganizerSpec : BaseSpecification<Event>
{
    public EventsByOrganizerSpec(UserId organizerId, int pageNumber, int pageSize)
    {
        AddAndCriteria(e => e.OrganizerId == organizerId);
        AddInclude(e => e.EventType);
        ApplyOrderByDescending(e => e.CreatedAt);

        // Page 1, Size 10 → Skip 0, Take 10
        // Page 2, Size 10 → Skip 10, Take 10
        // Page 3, Size 10 → Skip 20, Take 10
        ApplyPaging(
            skip: (pageNumber - 1) * pageSize,
            take: pageSize);

        ApplyNoTracking();
        ApplySplitQuery();
    }
}
