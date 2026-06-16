using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Notification.Domain.Errors;
using Modules.Notification.Domain.Repositories;
using NotificationEntity = Modules.Notification.Domain.Entities.Notification.Notification;

namespace Modules.Notification.Application.Notifications.MarkAsRead;

public sealed class MarkNotificationAsReadCommandHandler(
    INotificationRepository notificationRepository,
    IUserContext userContext)
    : ICommandHandler<MarkNotificationAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        NotificationEntity? notification = await notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification is null)
            return Result.Failure(NotificationErrors.NotFound);

        if (notification.RecipientId != userContext.UserId)
            return Result.Failure(NotificationErrors.Forbidden);

        if (notification.ReadAt is not null)
            return Result.Failure(NotificationErrors.AlreadyRead);

        notification.Read();
        await notificationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
