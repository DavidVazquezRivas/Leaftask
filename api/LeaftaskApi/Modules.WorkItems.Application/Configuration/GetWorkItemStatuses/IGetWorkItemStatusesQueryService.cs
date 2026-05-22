namespace Modules.WorkItems.Application.Configuration.GetWorkItemStatuses;

public interface IGetWorkItemStatusesQueryService
{
    Task<IReadOnlyList<WorkItemStatusDto>> GetWorkItemStatusesAsync(CancellationToken cancellationToken = default);
}
