using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.Organizations.SyncMemberPermissions;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Organizations.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.Organizations;

public sealed class OrganizationRolePermissionsUpdatedIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<OrganizationRolePermissionsUpdatedIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        OrganizationRolePermissionsUpdatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        foreach (AffectedMemberPermissions member in notification.AffectedMembers)
        {
            await sender.Send(new SyncMemberOrganizationPermissionsCommand(
                notification.OrganizationId,
                member.UserId,
                member.Permissions
                    .Select(p => new OrganizationPermissionEntryDto(p.PermissionName, p.Level))
                    .ToArray()), cancellationToken);
        }
    }

    protected override Guid GetFallbackMessageId(OrganizationRolePermissionsUpdatedIntegrationEvent notification) =>
        notification.OrganizationId;
}
