using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.Notifications.Create;
using Modules.Notification.Domain.Entities.Notification;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Projects.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.Projects;

public sealed class ProjectInvitationCreatedIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<ProjectInvitationCreatedIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        ProjectInvitationCreatedIntegrationEvent notification,
        CancellationToken cancellationToken) =>
        await sender.Send(new CreateNotificationCommand(
            NotificationType.ProjectInvitation,
            ContextId: notification.ProjectId,
            TargetId: notification.InvitationId,
            RecipientId: notification.InviteeId), cancellationToken);

    protected override Guid GetFallbackMessageId(ProjectInvitationCreatedIntegrationEvent notification) =>
        notification.InvitationId;
}
