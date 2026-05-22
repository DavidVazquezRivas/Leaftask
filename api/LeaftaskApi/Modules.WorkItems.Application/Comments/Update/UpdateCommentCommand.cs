using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;
using Modules.WorkItems.Application.Comments;

namespace Modules.WorkItems.Application.Comments.Update;

[RequireProjectPermission("Access Project")]
public sealed record UpdateCommentCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid CommentId,
    string? Content,
    IReadOnlyList<Guid>? AttachmentIds) : ICommand<Result<CommentDto>>, IProjectPermissionRequest;
