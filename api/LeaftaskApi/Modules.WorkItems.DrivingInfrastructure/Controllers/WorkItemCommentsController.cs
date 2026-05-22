using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.WorkItems.Application.Comments;
using Modules.WorkItems.Application.Comments.Add;
using Modules.WorkItems.Application.Comments.Delete;
using Modules.WorkItems.Application.Comments.List;
using Modules.WorkItems.Application.Comments.Update;
using Modules.WorkItems.DrivingInfrastructure.Models.Requests;

namespace Modules.WorkItems.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/projects/{projectId:guid}/work-items/{itemId:guid}/comments")]
public sealed class WorkItemCommentsController : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetComments(
        Guid projectId,
        Guid itemId,
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] string[]? sort = null,
        CancellationToken cancellationToken = default)
    {
        Result<PaginatedResult<CommentDto>> result = await Sender.Send(
            new GetCommentsQuery(projectId, itemId, limit, cursor, sort ?? []),
            cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        IReadOnlyList<SortMeta>? sortMeta = SortMetaParser.Parse(sort);
        PaginationMeta paginationMeta = new()
        {
            Limit = limit,
            NextCursor = result.Value.NextCursor,
            HasMore = result.Value.HasMore
        };

        return StatusCode(200, BuildSuccessResponse(result.Value.Items, sortMeta, paginationMeta));
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(
        Guid projectId,
        Guid itemId,
        [FromBody] AddCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<CommentDto> result = await Sender.Send(
            new AddCommentCommand(projectId, itemId, request.Content, request.AttachmentIds ?? []),
            cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return StatusCode(201, BuildSuccessResponse(result.Value));
    }

    [HttpPatch("{commentId:guid}")]
    public async Task<IActionResult> UpdateComment(
        Guid projectId,
        Guid itemId,
        Guid commentId,
        [FromBody] UpdateCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<CommentDto> result = await Sender.Send(
            new UpdateCommentCommand(projectId, itemId, commentId, request.Content, request.AttachmentIds),
            cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return StatusCode(200, BuildSuccessResponse(result.Value));
    }

    [HttpDelete("{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(
        Guid projectId,
        Guid itemId,
        Guid commentId,
        CancellationToken cancellationToken = default)
    {
        Result result = await Sender.Send(
            new DeleteCommentCommand(projectId, itemId, commentId),
            cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }
}
