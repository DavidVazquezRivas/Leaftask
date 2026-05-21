using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Comments.List;

public sealed class GetCommentsQueryHandler(
    IGetCommentsQueryService queryService,
    IWorkItemRepository workItemRepository)
    : IQueryHandler<GetCommentsQuery, Result<PaginatedResult<CommentDto>>>
{
    public async Task<Result<PaginatedResult<CommentDto>>> Handle(
        GetCommentsQuery request,
        CancellationToken cancellationToken)
    {
        bool exists = await workItemRepository.ExistsInProjectAsync(
            request.WorkItemId, request.ProjectId, cancellationToken);

        if (!exists)
        {
            return Result.Failure<PaginatedResult<CommentDto>>(WorkItemErrors.WorkItemNotFound);
        }

        PaginatedResult<CommentDto> result = await queryService.GetCommentsAsync(
            request.ProjectId,
            request.WorkItemId,
            request.Limit,
            request.Cursor,
            request.Sort,
            cancellationToken);

        return Result.Success(result);
    }
}
