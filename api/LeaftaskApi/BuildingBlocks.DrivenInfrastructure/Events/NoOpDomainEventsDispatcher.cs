using BuildingBlocks.Domain.Events;
using BuildingBlocks.Infrastructure.Events;

namespace BuildingBlocks.DrivenInfrastructure.Events;

public class NoOpDomainEventsDispatcher : IDomainEventsDispatcher
{
    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
