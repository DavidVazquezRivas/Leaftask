using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Events;
using Modules.Organizations.Integration;

namespace Modules.Organizations.Application.Events;

public sealed class OrganizationModuleEventMapper : IIntegrationEventMapper
{
    public IIntegrationEvent? Map(IDomainEvent domainEvent) =>
        domainEvent switch
        {
            OrganizationCreatedDomainEvent created => new OrganizationCreatedIntegrationEvent(
                created.OrganizationId,
                created.CreatorUserId),
            OrganizationDeletedDomainEvent deleted => new OrganizationDeletedIntegrationEvent(
                deleted.OrganizationId),
            OrganizationInvitationCreatedDomainEvent created => new OrganizationInvitationCreatedIntegrationEvent(
                created.OrganizationInvitationId,
                created.OrganizationId,
                created.UserId,
                created.OrganizationRoleId),
            OrganizationInvitationRespondedDomainEvent responded => new OrganizationInvitationRespondedIntegrationEvent(
                responded.OrganizationInvitationId,
                responded.OrganizationId,
                responded.UserId,
                responded.OrganizationRoleId,
                GetStatusName(responded.Status)),
            OrganizationPermissionActionRequestedDomainEvent requested => new
                OrganizationPermissionActionRequestedIntegrationEvent(
                    requested.OrganizationId,
                    requested.RequestedByUserId,
                    requested.PermissionName,
                    requested.ActionName,
                    requested.ActionPayload),
            OrganizationMemberJoinedDomainEvent joined => new OrganizationMemberJoinedIntegrationEvent(
                joined.OrganizationId,
                joined.UserId,
                joined.Permissions.Select(p => new Integration.OrganizationPermissionEntry(p.PermissionName, p.Level)).ToArray()),
            OrganizationMemberRoleChangedDomainEvent roleChanged => new OrganizationMemberRoleChangedIntegrationEvent(
                roleChanged.OrganizationId,
                roleChanged.UserId,
                roleChanged.NewPermissions.Select(p => new Integration.OrganizationPermissionEntry(p.PermissionName, p.Level)).ToArray()),
            OrganizationRolePermissionsUpdatedDomainEvent rolePermsUpdated => new OrganizationRolePermissionsUpdatedIntegrationEvent(
                rolePermsUpdated.OrganizationId,
                rolePermsUpdated.AffectedMembers
                    .Select(m => new Integration.AffectedMemberPermissions(
                        m.UserId,
                        m.Permissions.Select(p => new Integration.OrganizationPermissionEntry(p.PermissionName, p.Level)).ToArray()))
                    .ToArray()),
            OrganizationMemberRemovedDomainEvent removed => new OrganizationMemberRemovedIntegrationEvent(
                removed.OrganizationId,
                removed.UserId),
            _ => null
        };

    private static string GetStatusName(InvitationStatus status) =>
        status switch
        {
            InvitationStatus.Accepted => "accepted",
            InvitationStatus.Rejected => "rejected",
            InvitationStatus.Canceled => "canceled",
            InvitationStatus.Abandoned => "abandoned",
            InvitationStatus.Pending => "pending",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
}
