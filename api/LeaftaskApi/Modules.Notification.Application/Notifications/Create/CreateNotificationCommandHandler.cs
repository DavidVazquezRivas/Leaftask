using BuildingBlocks.Application.Commands;
using Modules.Notification.Domain.Repositories;
using NotificationEntity = Modules.Notification.Domain.Entities.Notification.Notification;

namespace Modules.Notification.Application.Notifications.Create;

public sealed class CreateNotificationCommandHandler(INotificationRepository notificationRepository)
    : ICommandHandler<CreateNotificationCommand>
{
    public async Task Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        NotificationEntity notification = NotificationEntity.Create(
            request.Type, request.ContextId, request.TargetId, request.RecipientId, request.ActorId);
        await notificationRepository.AddAsync(notification, cancellationToken);
        await notificationRepository.SaveChangesAsync(cancellationToken);
    }
}
