using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;

namespace Modules.WorkItems.Application.Events;

public sealed class WorkItemsModuleEventMapper : IIntegrationEventMapper
{
    public IIntegrationEvent? Map(IDomainEvent domainEvent) => null;
}
