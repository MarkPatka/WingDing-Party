using EventService.Domain;
using EventService.Domain.EventAggregate.ValueObjects;

namespace EventService.Application.Persistence.Specifications;

public class GetEventByTitleAndOrganizerSpecification : BaseSpecification<Event>
{
    public GetEventByTitleAndOrganizerSpecification(string title, OrganizerId OrganizerId)
    {
        //    x.EventType == request.EventType &&
        //    x.OrganizerId == request.OrganizerId
        AddCriteria(x => x.Title == title);
        AddCriteria(x => x.OrganizerId == OrganizerId);
        ApplyNoTracking();
    }
}
