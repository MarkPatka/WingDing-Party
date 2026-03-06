using EventService.Domain;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Application.Persistence.Specifications;

public class GetEventByTitleAndOrganizerSpecification : BaseSpecification<Event>
{
    public GetEventByTitleAndOrganizerSpecification(string title, UserId OrganizerId)
    {
        AddAndCriteria(x => x.Title == title);
        AddAndCriteria(x => x.OrganizerId == OrganizerId);
        ApplyNoTracking();
    }
}
