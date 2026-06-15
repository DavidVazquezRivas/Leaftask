using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Notification.Domain.Repositories;
using NotificationEntity = Modules.Notification.Domain.Entities.Notification.Notification;

namespace Modules.Notification.Application.Notifications.MarkAllAsRead;

public sealed class MarkAllNotificationsAsReadCommandHandler(
    INotificationRepository notificationRepository,
    IUserContext userContext)
    : ICommandHandler<MarkAllNotificationsAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        List<NotificationEntity> unread = (await notificationRepository.GetByRecipientIdAsync(userContext.UserId, cancellationToken))
            .Where(n => n.ReadAt is null)
            .ToList();

        foreach (NotificationEntity notification in unread)
            notification.Read();

        if (unread.Count > 0)
            await notificationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
