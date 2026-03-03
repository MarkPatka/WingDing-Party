using EventService.Application.Common.Extensions;
using EventService.Domain;
using System.Linq.Expressions;

namespace EventService.Application.Persistence.Specifications;

public class EventsByTextAndFiltersSpecification : BaseSpecification<Event>
{
    public EventsByTextAndFiltersSpecification(
        string text,
        string? eventType,
        string? city,
        DateTime? dateFrom,
        DateTime? dateTo,
        int pageNumber,
        int pageSize)
    {
        Expression<Func<Event, bool>> titleCriteria =
            e => e.Title.ToLower().Contains(text.ToLower());
        Expression<Func<Event, bool>> descriptionCriteria =
            e => e.Description.ToLower().Contains(text.ToLower());
        AddAndCriteria(titleCriteria.Or(descriptionCriteria));

        if (!string.IsNullOrWhiteSpace(eventType))
        {
            Expression<Func<Event, bool>> eventTypeNameCriteria =
                e => e.EventType.Name.ToLower().Contains(eventType.ToLower());
            Expression<Func<Event, bool>> eventTypeDescriptionCriteria =
                e => e.EventType.Description!.ToLower().Contains(eventType.ToLower());
            AddAndCriteria(eventTypeNameCriteria.Or(eventTypeDescriptionCriteria));
        }

        if (!string.IsNullOrWhiteSpace(city))
            AddAndCriteria(e => e.Location.City.ToLower().Contains(city.ToLower()));

        if (dateFrom.HasValue)
            AddAndCriteria(e => e.StartDate >= dateFrom.Value);

        if (dateTo.HasValue)
            AddAndCriteria(e => e.EndDate <= dateTo.Value);

        ApplyOrderBy(e => e.Title.ToLower().Contains(text.ToLower()) ? 0 : 1);
        ApplyOrderBy(e => e.StartDate);

        ApplyPaging(
            skip: (pageNumber - 1) * pageSize,
            take: pageSize);

        ApplyNoTracking();
    }
}
