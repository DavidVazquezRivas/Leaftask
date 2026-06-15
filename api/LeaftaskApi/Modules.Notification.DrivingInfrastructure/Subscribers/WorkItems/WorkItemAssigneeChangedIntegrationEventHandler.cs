using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.Notifications.Create;
using Modules.Notification.Domain.Entities.Notification;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.WorkItems.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.WorkItems;

public sealed class WorkItemAssigneeChangedIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<WorkItemAssigneeChangedIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        WorkItemAssigneeChangedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        if (notification.NewAssigneeId is null) return;

        await sender.Send(new CreateNotificationCommand(
            NotificationType.Assignment,
            ContextId: notification.ProjectId,
            TargetId: notification.WorkItemId,
            RecipientId: notification.NewAssigneeId.Value), cancellationToken);
    }

    protected override Guid GetFallbackMessageId(WorkItemAssigneeChangedIntegrationEvent notification) =>
        notification.WorkItemId;
}
