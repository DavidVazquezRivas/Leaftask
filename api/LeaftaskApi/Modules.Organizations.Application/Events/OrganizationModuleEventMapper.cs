using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;

namespace Modules.Organizations.Application.Events;

public sealed class OrganizationModuleEventMapper : IIntegrationEventMapper
{
    public IIntegrationEvent? Map(IDomainEvent domainEvent) =>
        domainEvent switch
        {
            _ => null
        };
}
