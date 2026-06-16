using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Notification.Domain.Repositories;
using NotificationEntity = Modules.Notification.Domain.Entities.Notification.Notification;

namespace Modules.Notification.Application.Notifications.DeleteByTarget;

public sealed class DeleteNotificationByTargetCommandHandler(INotificationRepository notificationRepository)
    : ICommandHandler<DeleteNotificationByTargetCommand, Result>
{
    public async Task<Result> Handle(DeleteNotificationByTargetCommand request, CancellationToken cancellationToken)
    {
        NotificationEntity? notification = await notificationRepository.GetByTargetIdAsync(request.TargetId, cancellationToken);
        if (notification is null)
            return Result.Success();

        notificationRepository.Delete(notification);
        await notificationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
