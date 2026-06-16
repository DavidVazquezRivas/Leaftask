using Modules.Notification.Domain.Entities.Approval.Permission;

namespace Modules.Notification.Domain.Repositories;

public interface IProjectPermissionReadModelRepository
{
    Task AddRangeAsync(IEnumerable<ProjectPermissionReadModel> permissions, CancellationToken ct = default);
    Task DeleteByMemberAsync(Guid userId, Guid projectId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid userId, Guid projectId, string permissionName, int level, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
