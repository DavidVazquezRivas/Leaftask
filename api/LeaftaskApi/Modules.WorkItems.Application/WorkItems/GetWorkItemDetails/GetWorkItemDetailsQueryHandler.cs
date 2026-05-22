using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.WorkItems;
using Modules.WorkItems.Domain.Errors;

namespace Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;

public sealed class GetWorkItemDetailsQueryHandler(IGetWorkItemDetailsQueryService queryService)
    : IQueryHandler<GetWorkItemDetailsQuery, Result<WorkItemDetailDto>>
{
    public async Task<Result<WorkItemDetailDto>> Handle(
        GetWorkItemDetailsQuery request,
        CancellationToken cancellationToken)
    {
        WorkItemDetailDto? result = await queryService.GetWorkItemDetailsAsync(
            request.ProjectId,
            request.WorkItemId,
            cancellationToken);

        return result is null
            ? Result.Failure<WorkItemDetailDto>(WorkItemErrors.WorkItemNotFound)
            : Result.Success(result);
    }
}
