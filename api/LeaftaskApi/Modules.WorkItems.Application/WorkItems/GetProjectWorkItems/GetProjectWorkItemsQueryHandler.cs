using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.WorkItems.GetProjectWorkItems;

public sealed class GetProjectWorkItemsQueryHandler(IGetProjectWorkItemsQueryService queryService)
    : IQueryHandler<GetProjectWorkItemsQuery, Result<PaginatedResult<WorkItemListDto>>>
{
    public async Task<Result<PaginatedResult<WorkItemListDto>>> Handle(
        GetProjectWorkItemsQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResult<WorkItemListDto> result = await queryService.GetProjectWorkItemsAsync(
            request.ProjectId,
            request.Limit,
            request.Cursor,
            request.Sort,
            cancellationToken);

        return Result.Success(result);
    }
}
