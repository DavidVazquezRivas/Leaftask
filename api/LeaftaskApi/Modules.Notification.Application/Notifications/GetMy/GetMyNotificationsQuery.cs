using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Notification.Application.Notifications.GetMy;

public sealed class GetMyNotificationsQuery : IPaginatedQuery<Result<PaginatedResult<NotificationDto>>>
{
    public int Limit { get; init; } = 10;
    public string? Cursor { get; init; }
    public IReadOnlyCollection<string> Sort { get; init; } = [];
    public string Status { get; init; } = "all";
}
