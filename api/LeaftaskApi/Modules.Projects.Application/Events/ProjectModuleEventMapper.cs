using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using Modules.Projects.Domain.Events;
using Modules.Projects.Integration;

namespace Modules.Projects.Application.Events;

public sealed class ProjectModuleEventMapper : IIntegrationEventMapper
{
    public IIntegrationEvent? Map(IDomainEvent domainEvent) => domainEvent switch
    {
        ProjectCreatedDomainEvent e => new ProjectCreatedIntegrationEvent(e.ProjectId, e.Abbreviation),
        ProjectDeletedDomainEvent e => new ProjectDeletedIntegrationEvent(e.ProjectId),
        _ => null
    };
}
