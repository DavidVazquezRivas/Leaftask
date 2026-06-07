using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using Modules.Agents.Domain.Events;
using Modules.Agents.Integration;

namespace Modules.Agents.Application.Events;

public sealed class AgentsModuleEventMapper : IIntegrationEventMapper
{
    public IIntegrationEvent? Map(IDomainEvent domainEvent) =>
        domainEvent switch
        {
            AgentCreatedDomainEvent e => new AgentCreatedIntegrationEvent(e.AgentId, e.Name, e.ProjectId, e.RoleId),
            AgentDeletedDomainEvent e => new AgentDeletedIntegrationEvent(e.AgentId, e.ProjectId),
            _ => null
        };
}
