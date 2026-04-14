using EventService.Domain.Common.Abstract;
using EventService.Infrastructure.EventSourcing.Messaging;
using MediatR;

namespace EventService.Infrastructure.EventSourcing.Extensions;

public static class KafkaEventEnvelopeExtensions
{
    private static readonly Dictionary<string, Type> DomainEvents = ScanDomainEvents();

    private static Dictionary<string, Type> ScanDomainEvents()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IDomainEvent).IsAssignableFrom(t) && !t.IsAbstract)
            .ToDictionary(
                t => t.Name.Replace("DomainEvent", ""),
                t => t,
                StringComparer.OrdinalIgnoreCase);
    }

    public static IDomainEvent MapToDomainEvent(this KafkaEventEnvelope envelope)
    {
        if (!DomainEvents.TryGetValue(envelope.EventType, out var eventType))
            throw new NotSupportedException(
                $"DomainEvent {envelope.EventType} not found");

        var constructor = eventType.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .First();

        var args = constructor.GetParameters()
            .Select(p => envelope.Data.TryGetProperty(p.Name!, out var prop)
                ? prop.ParseProperty(p.ParameterType)
                : Activator.CreateInstance(p.ParameterType))
            .ToArray();

        return (IDomainEvent)Activator.CreateInstance(eventType, args)!;
    }

    public static IRequest MapToCommand(this KafkaEventEnvelope envelope)
    {
        var domainEvent = envelope.MapToDomainEvent();

        // Convention: EventCreated -> CreateEventCommand
        var eventName = domainEvent.GetType().Name.Replace("DomainEvent", "");
        var commandName = $"{eventName}Command";

        var commandType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == commandName
                && typeof(IRequest).IsAssignableFrom(t));

        if (commandType == null)
            throw new NotSupportedException($"Command {commandName} not found");

        // Copy properties по имени
        var command = Activator.CreateInstance(commandType)!;
        CopyProperties(domainEvent, command);

        return (IRequest)command!;
    }

    private static void CopyProperties(object source, object target)
    {
        var sourceProps = source.GetType().GetProperties();
        var targetProps = target.GetType().GetProperties()
            .Where(p => p.CanWrite);

        foreach (var targetProp in targetProps)
        {
            var sourceProp = sourceProps.FirstOrDefault(p => p.Name == targetProp.Name);
            if (sourceProp != null)
            {
                var value = sourceProp.GetValue(source);
                targetProp.SetValue(target, value);
            }
        }
    }
}
