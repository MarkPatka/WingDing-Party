namespace EventService.Application.EventSourcing;

public interface ITopicProvisioner
{
    Task ProvisionAsync(CancellationToken cancellationToken = default);
}
