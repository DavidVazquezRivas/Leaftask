using NotificationEntity = Modules.Notification.Domain.Entities.Notification.Notification;

namespace Modules.Notification.Domain.Repositories;

public interface INotificationRepository
{
    Task AddAsync(NotificationEntity notification, CancellationToken ct = default);
    Task<NotificationEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<NotificationEntity>> GetByRecipientIdAsync(Guid recipientId, CancellationToken ct = default);
    Task<NotificationEntity?> GetByTargetIdAsync(Guid targetId, CancellationToken ct = default);
    void Delete(NotificationEntity notification);
    Task SaveChangesAsync(CancellationToken ct = default);
}
