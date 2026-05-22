using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;

namespace Modules.WorkItems.Application.Comments.Delete;

[RequireProjectPermission("Access Project")]
public sealed record DeleteCommentCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid CommentId) : ICommand<Result>, IProjectPermissionRequest;
