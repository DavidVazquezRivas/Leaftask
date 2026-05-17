namespace Modules.WorkItems.Application.Configuration.GetWorkItemTypes;

public interface IGetWorkItemTypesQueryService
{
    Task<IReadOnlyList<WorkItemTypeDto>> GetWorkItemTypesAsync(CancellationToken cancellationToken = default);
}
