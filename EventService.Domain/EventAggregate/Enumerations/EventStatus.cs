using EventService.Domain.Common.Abstract;

namespace EventService.Domain.EventAggregate.Enumerations;

public sealed class EventStatus : Enumeration
{
    public static readonly EventStatus Draft = new(1, nameof(Draft), "Черновик");
    public static readonly EventStatus Published = new(2, nameof(Published), "Опубликовано");
    public static readonly EventStatus Cancelled = new(3, nameof(Cancelled), "Отменено");
    public static readonly EventStatus Completed = new(4, nameof(Completed), "Завершено");

    private EventStatus(int id, string name, string? description = null)
        : base(id, name, description) { }
}
