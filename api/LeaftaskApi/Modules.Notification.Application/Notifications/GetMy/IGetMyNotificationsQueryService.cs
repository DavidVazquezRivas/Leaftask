using BuildingBlocks.Application.Queries;

namespace Modules.Notification.Application.Notifications.GetMy;

public interface IGetMyNotificationsQueryService
{
    Task<PaginatedResult<NotificationDto>> GetMyNotificationsAsync(
        Guid recipientId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        string status,
        CancellationToken cancellationToken = default);
}
