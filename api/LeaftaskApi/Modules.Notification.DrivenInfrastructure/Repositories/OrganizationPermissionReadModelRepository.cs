using Microsoft.EntityFrameworkCore;
using Modules.Notification.Domain.Entities.Approval.Permission;
using Modules.Notification.Domain.Repositories;
using Modules.Notification.DrivenInfrastructure.Persistence;

namespace Modules.Notification.DrivenInfrastructure.Repositories;

public sealed class OrganizationPermissionReadModelRepository(NotificationsDbContext dbContext)
    : IOrganizationPermissionReadModelRepository
{
    public async Task AddRangeAsync(IEnumerable<OrganizationPermissionReadModel> permissions,
        CancellationToken ct = default) =>
        await dbContext.OrganizationPermissionReadModels.AddRangeAsync(permissions, ct);

    public async Task<bool> ExistsAsync(Guid userId, Guid organizationId, string permissionName, int level,
        CancellationToken ct = default) =>
        await dbContext.OrganizationPermissionReadModels
            .AsNoTracking()
            .AnyAsync(p => p.UserId == userId && p.OrganizationId == organizationId
                           && p.PermissionName == permissionName && p.Level == level, ct);

    public async Task DeleteByMemberAsync(Guid userId, Guid organizationId, CancellationToken ct = default)
    {
        List<OrganizationPermissionReadModel> existing = await dbContext.OrganizationPermissionReadModels
            .Where(p => p.UserId == userId && p.OrganizationId == organizationId)
            .ToListAsync(ct);

        dbContext.OrganizationPermissionReadModels.RemoveRange(existing);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await dbContext.SaveChangesAsync(ct);
}
