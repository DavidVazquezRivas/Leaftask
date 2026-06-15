using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.Notifications.DeleteByTarget;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Organizations.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.Organizations;

public sealed class OrganizationInvitationRespondedIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<OrganizationInvitationRespondedIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        OrganizationInvitationRespondedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        if (!notification.Status.Equals("canceled", StringComparison.OrdinalIgnoreCase))
            return;

        await sender.Send(new DeleteNotificationByTargetCommand(notification.OrganizationInvitationId), cancellationToken);
    }

    protected override Guid GetFallbackMessageId(OrganizationInvitationRespondedIntegrationEvent notification) =>
        notification.OrganizationInvitationId;
}
