using Modules.WorkItems.Domain.Entities;

namespace Modules.WorkItems.Domain.Repositories;

public interface IWorkItemRepository
{
    Task<int> GetNextCodeAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<bool> ExistsInProjectAsync(Guid workItemId, Guid projectId, CancellationToken cancellationToken = default);
    Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default);
    Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkItem?> GetByIdTrackedAsync(Guid id, Guid projectId, CancellationToken cancellationToken = default);
    void Remove(WorkItem workItem);
    Task AddActivityLogAsync(ActivityLog activityLog, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
