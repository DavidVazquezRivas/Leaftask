using BuildingBlocks.Application.Queries;

namespace Modules.WorkItems.Application.Comments.List;

public interface IGetCommentsQueryService
{
    Task<PaginatedResult<CommentDto>> GetCommentsAsync(
        Guid projectId,
        Guid workItemId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default);
}
