using System.Threading;
using System.Threading.Tasks;
using UserService.Application.IntegrationEvents;

namespace UserService.Infrastructure.Messaging;

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