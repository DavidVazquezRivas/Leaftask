using Modules.WorkItems.Domain.Entities;

namespace Modules.WorkItems.Domain.Repositories;

public interface ICommentRepository
{
    Task AddAsync(WorkItemComment comment, CancellationToken cancellationToken = default);
    Task<WorkItemComment?> GetByIdTrackedAsync(Guid commentId, Guid workItemId, CancellationToken cancellationToken = default);
    void Remove(WorkItemComment comment);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
