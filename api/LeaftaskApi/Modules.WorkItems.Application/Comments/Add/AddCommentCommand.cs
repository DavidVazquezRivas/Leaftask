using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;
using Modules.WorkItems.Application.Comments;

namespace Modules.WorkItems.Application.Comments.Add;

[RequireProjectPermission("work-items.comment")]
public sealed record AddCommentCommand(
    Guid ProjectId,
    Guid WorkItemId,
    string Content,
    IReadOnlyList<Guid> AttachmentIds) : ICommand<Result<CommentDto>>, IProjectPermissionRequest;
