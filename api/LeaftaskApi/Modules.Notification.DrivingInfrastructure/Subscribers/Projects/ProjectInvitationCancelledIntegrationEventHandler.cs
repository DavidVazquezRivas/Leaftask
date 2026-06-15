using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.Notifications.DeleteByTarget;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Projects.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.Projects;

public sealed class ProjectInvitationCancelledIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<ProjectInvitationCancelledIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        ProjectInvitationCancelledIntegrationEvent notification,
        CancellationToken cancellationToken) =>
        await sender.Send(new DeleteNotificationByTargetCommand(notification.InvitationId), cancellationToken);

    protected override Guid GetFallbackMessageId(ProjectInvitationCancelledIntegrationEvent notification) =>
        notification.InvitationId;
}
