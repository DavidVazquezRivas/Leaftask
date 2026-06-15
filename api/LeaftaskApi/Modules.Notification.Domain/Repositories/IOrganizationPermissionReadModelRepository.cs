using Modules.Notification.Domain.Entities.Approval.Permission;

namespace Modules.Notification.Domain.Repositories;

public interface IOrganizationPermissionReadModelRepository
{
    Task AddRangeAsync(IEnumerable<OrganizationPermissionReadModel> permissions, CancellationToken ct = default);
    Task DeleteByMemberAsync(Guid userId, Guid organizationId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid userId, Guid organizationId, string permissionName, int level, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
