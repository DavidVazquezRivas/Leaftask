using System.ComponentModel;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.WorkItems.Application.Comments;
using Modules.WorkItems.Application.Comments.Add;
using Modules.WorkItems.Application.Comments.Delete;
using Modules.WorkItems.Application.Comments.List;
using Modules.WorkItems.Application.Comments.Update;

namespace Modules.WorkItems.DrivingInfrastructure.Tools;

public class WorkItemCommentsAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetWorkItemComments")]
    [Description("Retrieves the comments posted on a specific work item.")]
    public async Task<string> GetCommentsAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the work item.")]
        Guid workItemId,
        [Description("Max results. Default is 10.")]
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        Result<PaginatedResult<CommentDto>> result =
            await sender.Send(new GetCommentsQuery(projectId, workItemId, limit, null, []), cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(GetCommentsAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value.Items);
    }

    [KernelFunction("AddWorkItemComment")]
    [Description("Posts a new comment on a work item.")]
    public async Task<string> AddCommentAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the work item.")]
        Guid workItemId,
        [Description("The text content of the comment.")]
        string content,
        CancellationToken cancellationToken = default)
    {
        Result<CommentDto> result =
            await sender.Send(new AddCommentCommand(projectId, workItemId, content, []), cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(AddCommentAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("UpdateWorkItemComment")]
    [Description("Updates the content of an existing comment on a work item.")]
    public async Task<string> UpdateCommentAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the work item.")]
        Guid workItemId,
        [Description(
            "The unique identifier (GUID) of the comment to update. Find this ID first using 'GetWorkItemComments'.")]
        Guid commentId,
        [Description("The new content for the comment.")]
        string content,
        CancellationToken cancellationToken = default)
    {
        Result<CommentDto> result = await sender.Send(
            new UpdateCommentCommand(projectId, workItemId, commentId, content, null), cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(UpdateCommentAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("DeleteWorkItemComment")]
    [Description("Deletes a comment from a work item. Use carefully.")]
    public async Task<string> DeleteCommentAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the work item.")]
        Guid workItemId,
        [Description(
            "The unique identifier (GUID) of the comment to delete. Find this ID first using 'GetWorkItemComments'.")]
        Guid commentId,
        CancellationToken cancellationToken = default)
    {
        Result result =
            await sender.Send(new DeleteCommentCommand(projectId, workItemId, commentId), cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(DeleteCommentAsync), result.Error.Description);

        return formatter.FormatMessage("Comment deleted successfully.");
    }
}
