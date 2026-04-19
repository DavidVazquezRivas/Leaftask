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
