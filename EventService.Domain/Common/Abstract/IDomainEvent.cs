using MediatR;

namespace EventService.Domain.Common.Abstract;

public interface IDomainEvent : INotification
{
    public DateTime OccurredOn { get; }
}
