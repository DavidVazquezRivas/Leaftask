using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.WorkItems.Application.Comments.Delete;

[RequireProjectPermission("work-items.comment")]
public sealed record DeleteCommentCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid CommentId) : ICommand<Result>, IProjectPermissionRequest;
