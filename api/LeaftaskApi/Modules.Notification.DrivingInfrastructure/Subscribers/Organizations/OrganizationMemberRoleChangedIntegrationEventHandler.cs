using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.Organizations.SyncMemberPermissions;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Organizations.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.Organizations;

public sealed class OrganizationMemberRoleChangedIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<OrganizationMemberRoleChangedIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        OrganizationMemberRoleChangedIntegrationEvent notification,
        CancellationToken cancellationToken) =>
        await sender.Send(new SyncMemberOrganizationPermissionsCommand(
            notification.OrganizationId,
            notification.UserId,
            notification.NewPermissions
                .Select(p => new OrganizationPermissionEntryDto(p.PermissionName, p.Level))
                .ToArray()), cancellationToken);

    protected override Guid GetFallbackMessageId(OrganizationMemberRoleChangedIntegrationEvent notification) =>
        notification.UserId;
}
