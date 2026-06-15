using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.Organizations.RemoveMemberPermissions;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Organizations.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.Organizations;

public sealed class OrganizationMemberRemovedIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<OrganizationMemberRemovedIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        OrganizationMemberRemovedIntegrationEvent notification,
        CancellationToken cancellationToken) =>
        await sender.Send(new RemoveMemberOrganizationPermissionsCommand(
            notification.OrganizationId,
            notification.UserId), cancellationToken);

    protected override Guid GetFallbackMessageId(OrganizationMemberRemovedIntegrationEvent notification) =>
        notification.UserId;
}
