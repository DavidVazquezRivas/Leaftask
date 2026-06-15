using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Notification.Application.Notifications.GetMy;

public sealed class GetMyNotificationsQueryHandler(
    IGetMyNotificationsQueryService service,
    IUserContext userContext)
    : IQueryHandler<GetMyNotificationsQuery, Result<PaginatedResult<NotificationDto>>>
{
    public async Task<Result<PaginatedResult<NotificationDto>>> Handle(
        GetMyNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResult<NotificationDto> notifications = await service.GetMyNotificationsAsync(
            userContext.UserId,
            request.Limit,
            request.Cursor,
            request.Sort,
            request.Status,
            cancellationToken);

        return Result.Success(notifications);
    }
}
