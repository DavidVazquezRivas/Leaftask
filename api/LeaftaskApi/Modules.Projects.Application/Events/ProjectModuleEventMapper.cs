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
        FieldCreatedDomainEvent e => new FieldCreatedIntegrationEvent(e.FieldId, e.Name, e.IsOptional, e.FieldTypeId, e.WorkItemTypeIds),
        FieldUpdatedDomainEvent e => new FieldUpdatedIntegrationEvent(e.FieldId, e.Name, e.IsOptional, e.FieldTypeId, e.WorkItemTypeIds),
        FieldDeletedDomainEvent e => new FieldDeletedIntegrationEvent(e.FieldId),
        ProjectInvitationCreatedDomainEvent e => new ProjectInvitationCreatedIntegrationEvent(e.InvitationId, e.ProjectId, e.InviteeId),
        ProjectInvitationCancelledDomainEvent e => new ProjectInvitationCancelledIntegrationEvent(e.InvitationId, e.ProjectId, e.InviteeId),
        ProjectMemberJoinedDomainEvent e => new ProjectMemberJoinedIntegrationEvent(
            e.ProjectId, e.UserId,
            e.Permissions.Select(p => new ProjectPermissionEntryDto(p.PermissionName, p.Level)).ToArray()),
        ProjectPermissionActionRequestedDomainEvent e => new ProjectPermissionActionRequestedIntegrationEvent(
            e.ProjectId, e.RequestedByUserId, e.PermissionName, e.ActionName, e.ActionPayload),
        _ => null
    };
}
