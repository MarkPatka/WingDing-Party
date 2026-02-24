using System.Reflection;
using ClubService.Application.Common.Attributes;
using ClubService.Application.IntegrationEvents;

namespace ClubService.Infrastructure.Messaging.Extensions;

public static class IntegrationEventExtensions
{
    public static string GetAggregateName(this IIntegrationEvent evt)
    {
        var attr = evt.GetType().GetCustomAttribute<AggregateAttribute>();
        if (attr == null)
            throw new InvalidOperationException($"Event {evt.GetType().Name} does not have [Aggregate] attribute");
        return attr.Name;
    }
}