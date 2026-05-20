using Modules.WorkItems.Domain.Entities;

namespace Modules.WorkItems.Domain.Repositories;

public interface IWorkLogRepository
{
    Task AddAsync(WorkLog workLog, CancellationToken cancellationToken = default);
    Task<WorkLog?> GetByIdTrackedAsync(Guid workLogId, Guid workItemId, CancellationToken cancellationToken = default);
    void Remove(WorkLog workLog);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
