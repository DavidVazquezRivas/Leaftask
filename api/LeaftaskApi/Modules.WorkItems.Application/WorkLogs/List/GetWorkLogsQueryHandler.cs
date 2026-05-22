using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.WorkLogs.List;

public sealed class GetWorkLogsQueryHandler(
    IGetWorkLogsQueryService queryService,
    IWorkItemRepository workItemRepository)
    : IQueryHandler<GetWorkLogsQuery, Result<PaginatedResult<WorkLogDto>>>
{
    public async Task<Result<PaginatedResult<WorkLogDto>>> Handle(
        GetWorkLogsQuery request,
        CancellationToken cancellationToken)
    {
        bool exists = await workItemRepository.ExistsInProjectAsync(
            request.WorkItemId, request.ProjectId, cancellationToken);

        if (!exists)
        {
            return Result.Failure<PaginatedResult<WorkLogDto>>(WorkItemErrors.WorkItemNotFound);
        }

        PaginatedResult<WorkLogDto> result = await queryService.GetWorkLogsAsync(
            request.ProjectId,
            request.WorkItemId,
            request.Limit,
            request.Cursor,
            request.Sort,
            cancellationToken);

        return Result.Success(result);
    }
}
