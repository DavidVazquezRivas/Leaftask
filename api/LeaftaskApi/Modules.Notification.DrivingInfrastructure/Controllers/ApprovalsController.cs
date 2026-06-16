using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Mvc;
using Modules.Notification.Application.ApprovalRequests.AddComment;
using Modules.Notification.Application.ApprovalRequests.GetMy;
using Modules.Notification.Application.ApprovalRequests.UpdateStatus;
using Modules.Notification.DrivingInfrastructure.Models.Requests;

namespace Modules.Notification.DrivingInfrastructure.Controllers;

[Route("api/v1/approvals")]
public sealed class ApprovalsController : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyApprovals(
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        GetMyApprovalsQuery query = new()
        {
            Limit = limit,
            Cursor = cursor
        };

        Result<PaginatedResult<ApprovalDto>> result = await Sender.Send(query, cancellationToken);

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

    [HttpPatch("{approvalId:guid}")]
    public async Task<IActionResult> UpdateApprovalStatus(
        Guid approvalId,
        [FromBody] UpdateApprovalStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        UpdateApprovalStatusCommand command = new(approvalId, request.Status);

        Result<ApprovalDto> result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return HandleFailure(result.Error);

        return StatusCode(200, BuildSuccessResponse(result.Value));
    }

    [HttpPost("{approvalId:guid}/comments")]
    public async Task<IActionResult> AddComment(
        Guid approvalId,
        [FromBody] AddApprovalCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        AddApprovalCommentCommand command = new(approvalId, request.Content);

        Result<ApprovalCommentDto> result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return HandleFailure(result.Error);

        return StatusCode(201, BuildSuccessResponse(result.Value));
    }
}
