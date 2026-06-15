using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.Notifications.Create;
using Modules.Notification.Domain.Entities.Notification;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Organizations.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.Organizations;

public sealed class OrganizationInvitationCreatedIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<OrganizationInvitationCreatedIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        OrganizationInvitationCreatedIntegrationEvent notification,
        CancellationToken cancellationToken) =>
        await sender.Send(new CreateNotificationCommand(
            NotificationType.Invitation,
            ContextId: notification.OrganizationId,
            TargetId: notification.OrganizationInvitationId,
            RecipientId: notification.UserId), cancellationToken);

    protected override Guid GetFallbackMessageId(OrganizationInvitationCreatedIntegrationEvent notification) =>
        notification.OrganizationInvitationId;
}
