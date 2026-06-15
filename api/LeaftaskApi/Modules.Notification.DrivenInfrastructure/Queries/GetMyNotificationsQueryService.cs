using System.Globalization;
using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.Notification.Application.Notifications.GetMy;
using Modules.Notification.Domain.Entities.Notification;
using Modules.Notification.DrivenInfrastructure.Persistence;

namespace Modules.Notification.DrivenInfrastructure.Queries;

public sealed class GetMyNotificationsQueryService(NotificationsDbContext dbContext)
    : IGetMyNotificationsQueryService
{
    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<NotificationRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<NotificationRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", row => row.Id, value => Guid.Parse(value), value => ((Guid)value).ToString("D")),
            ["timestamp"] = new("timestamp", row => row.CreatedAt,
                value => DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                value => ((DateTime)value).ToString("O", CultureInfo.InvariantCulture))
        };

    private static readonly IReadOnlyList<string> DefaultSort = ["timestamp:desc", "id:desc"];

    public async Task<PaginatedResult<NotificationDto>> GetMyNotificationsAsync(
        Guid recipientId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        string status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Domain.Entities.Notification.Notification> query = dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.RecipientId == recipientId);

        query = status.ToUpperInvariant() switch
        {
            "READ" => query.Where(n => n.ReadAt != null),
            "UNREAD" => query.Where(n => n.ReadAt == null),
            _ => query
        };

        List<Domain.Entities.Notification.Notification> notifications =
            await query.ToListAsync(cancellationToken);

        List<Guid> actorIds = notifications
            .Where(n => n.ActorId.HasValue)
            .Select(n => n.ActorId!.Value)
            .Distinct()
            .ToList();

        Dictionary<Guid, string> actorNames = actorIds.Count > 0
            ? await dbContext.UserReadModels
                .AsNoTracking()
                .Where(u => actorIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.FirstName + " " + u.LastName, cancellationToken)
            : [];

        List<NotificationRow> rows = notifications
            .Select(n => new NotificationRow(
                n.Id,
                n.Type,
                n.ContextId,
                n.TargetId,
                n.ActorId,
                n.ActorId.HasValue ? actorNames.GetValueOrDefault(n.ActorId.Value) : null,
                n.CreatedAt,
                n.ReadAt))
            .ToList();

        return CursorPaginationHelper.Paginate(
            rows,
            limit,
            cursor,
            sort,
            SortFields,
            DefaultSort,
            row => new NotificationDto(
                row.Id,
                row.Type.ToString(),
                new SimpleReferenceDto(row.ContextId, row.ContextId.ToString()),
                new SimpleReferenceDto(row.TargetId, row.TargetId.ToString()),
                row.CreatedAt,
                row.ActorId.HasValue && row.ActorName != null
                    ? new SimpleReferenceDto(row.ActorId.Value, row.ActorName)
                    : null,
                row.ReadAt.HasValue));
    }

    private sealed record NotificationRow(
        Guid Id,
        NotificationType Type,
        Guid ContextId,
        Guid TargetId,
        Guid? ActorId,
        string? ActorName,
        DateTime CreatedAt,
        DateTime? ReadAt);
}
