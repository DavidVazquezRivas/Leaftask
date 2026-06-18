using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.WorkItems.Application.Comments.List;

[RequireProjectPermission("Access Project")]
public sealed record GetCommentsQuery(
    Guid ProjectId,
    Guid WorkItemId,
    int Limit,
    string? Cursor,
    IReadOnlyCollection<string> Sort) : IQuery<Result<PaginatedResult<CommentDto>>>, IProjectPermissionRequest;
