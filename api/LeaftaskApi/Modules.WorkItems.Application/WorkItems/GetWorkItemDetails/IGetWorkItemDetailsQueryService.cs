using Modules.WorkItems.Application.WorkItems;

namespace Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;

public interface IGetWorkItemDetailsQueryService
{
    Task<WorkItemDetailDto?> GetWorkItemDetailsAsync(
        Guid projectId,
        Guid workItemId,
        CancellationToken cancellationToken = default);
}
