using System;

namespace UserService.Application.IntegrationEvents;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOnUtc { get; }
}