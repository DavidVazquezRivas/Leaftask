using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.Notifications.Create;
using Modules.Notification.Domain.Entities.Notification;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.WorkItems.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.WorkItems;

public sealed class UsersMentionedInCommentIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<UsersMentionedInCommentIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        UsersMentionedInCommentIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        foreach (Guid userId in notification.MentionedUserIds)
        {
            if (userId == notification.AuthorId) continue;

            // contextId = workItem (where the comment lives), targetId = the comment itself
            await sender.Send(new CreateNotificationCommand(
                NotificationType.Mention,
                ContextId: notification.WorkItemId,
                TargetId: notification.CommentId,
                RecipientId: userId,
                ActorId: notification.AuthorId), cancellationToken);
        }
    }

    protected override Guid GetFallbackMessageId(UsersMentionedInCommentIntegrationEvent notification) =>
        notification.CommentId;
}
