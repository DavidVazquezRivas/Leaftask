using Microsoft.EntityFrameworkCore;
using Modules.Notification.Domain.Repositories;
using Modules.Notification.DrivenInfrastructure.Persistence;

namespace Modules.Notification.DrivenInfrastructure.Repositories;

public sealed class NotificationRepository(NotificationsDbContext dbContext) : INotificationRepository
{
    public async Task AddAsync(Domain.Entities.Notification.Notification notification,
        CancellationToken ct = default) =>
        await dbContext.Notifications.AddAsync(notification, ct);

    public async Task<Domain.Entities.Notification.Notification?> GetByIdAsync(Guid id,
        CancellationToken ct = default) =>
        await dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<List<Domain.Entities.Notification.Notification>> GetByRecipientIdAsync(Guid recipientId,
        CancellationToken ct = default) =>
        await dbContext.Notifications
            .Where(n => n.RecipientId == recipientId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

    public async Task<Domain.Entities.Notification.Notification?> GetByTargetIdAsync(Guid targetId,
        CancellationToken ct = default) =>
        await dbContext.Notifications.FirstOrDefaultAsync(n => n.TargetId == targetId, ct);

    public void Delete(Domain.Entities.Notification.Notification notification) =>
        dbContext.Notifications.Remove(notification);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await dbContext.SaveChangesAsync(ct);
}
