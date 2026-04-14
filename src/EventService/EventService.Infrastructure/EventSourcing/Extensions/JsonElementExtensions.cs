using EventService.Domain.EventAggregate.ValueObjects;
using System.Text.Json;

namespace EventService.Infrastructure.EventSourcing.Extensions;

public static class JsonElementExtensions
{
    private static readonly Dictionary<Type, Func<JsonElement, object>> Parsers = new()
    {
        [typeof(string)] = e => e.GetString()!,
        [typeof(Guid)] = e => Guid.Parse(e.GetString()!),
        [typeof(Guid?)] = e => e.ValueKind == JsonValueKind.Null ? null! 
            : Guid.Parse(e.GetString()!),
        [typeof(DateTime)] = e => DateTime.Parse(e.GetString()!),
        [typeof(DateTime?)] = e => e.ValueKind == JsonValueKind.Null ? null!
            : DateTime.Parse(e.GetString()!),
        [typeof(int)] = e => e.GetInt32(),
        [typeof(int?)] = e => e.ValueKind == JsonValueKind.Null ? null!
            : e.GetInt32(),
        [typeof(EventId)] = e => EventId.Create(Guid.Parse(e.GetString()!)),
        [typeof(ParticipantId)] = e => ParticipantId.Create(Guid.Parse(e.GetString()!))
    };

    public static object ParseProperty(this JsonElement element, Type targetType)
    {
        if (Parsers.TryGetValue(targetType, out var parser))
            return parser(element);

        throw new NotSupportedException($"Type {targetType.Name} not suppported");
    }
}
