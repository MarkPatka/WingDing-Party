using ClubService.Application.IntegrationEvents;

namespace ClubService.Infrastructure.Messaging;

public interface IIntegrationEventDispatcher
{
    Task DispatchAsync(
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken);

    Task DispatchToDeadLetterAsync(
        IIntegrationEvent integrationEvent,
        Exception exception,
        CancellationToken cancellationToken);
}