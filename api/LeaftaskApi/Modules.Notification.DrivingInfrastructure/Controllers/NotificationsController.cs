using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Mvc;
using Modules.Notification.Application.Notifications.GetMy;
using Modules.Notification.Application.Notifications.MarkAllAsRead;
using Modules.Notification.Application.Notifications.MarkAsRead;

namespace Modules.Notification.DrivingInfrastructure.Controllers;

[Route("api/v1/notifications")]
public sealed class NotificationsController : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] string status = "all",
        CancellationToken cancellationToken = default)
    {
        GetMyNotificationsQuery query = new()
        {
            Limit = limit,
            Cursor = cursor,
            Status = status
        };

        Result<PaginatedResult<NotificationDto>> result = await Sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return HandleFailure(result.Error);

        PaginationMeta paginationMeta = new()
        {
            Limit = limit,
            NextCursor = result.Value.NextCursor,
            HasMore = result.Value.HasMore
        };

        return StatusCode(200, BuildSuccessResponse(result.Value.Items, null, paginationMeta));
    }

    [HttpPatch("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId, CancellationToken cancellationToken = default) =>
        HandleResult(await Sender.Send(new MarkNotificationAsReadCommand(notificationId), cancellationToken));

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default) =>
        HandleResult(await Sender.Send(new MarkAllNotificationsAsReadCommand(), cancellationToken));
}
