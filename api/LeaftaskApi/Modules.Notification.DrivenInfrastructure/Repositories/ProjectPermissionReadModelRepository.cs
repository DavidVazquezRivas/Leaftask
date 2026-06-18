using Microsoft.EntityFrameworkCore;
using Modules.Notification.Domain.Entities.Approval.Permission;
using Modules.Notification.Domain.Repositories;
using Modules.Notification.DrivenInfrastructure.Persistence;

namespace Modules.Notification.DrivenInfrastructure.Repositories;

public sealed class ProjectPermissionReadModelRepository(NotificationsDbContext dbContext)
    : IProjectPermissionReadModelRepository
{
    public async Task AddRangeAsync(IEnumerable<ProjectPermissionReadModel> permissions, CancellationToken ct = default) =>
        await dbContext.ProjectPermissionReadModels.AddRangeAsync(permissions, ct);

    public async Task DeleteByMemberAsync(Guid userId, Guid projectId, CancellationToken ct = default)
    {
        List<ProjectPermissionReadModel> existing = await dbContext.ProjectPermissionReadModels
            .Where(p => p.UserId == userId && p.ProjectId == projectId)
            .ToListAsync(ct);
        dbContext.ProjectPermissionReadModels.RemoveRange(existing);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid projectId, string permissionName, int level, CancellationToken ct = default) =>
        await dbContext.ProjectPermissionReadModels
            .AsNoTracking()
            .AnyAsync(p => p.UserId == userId && p.ProjectId == projectId && p.PermissionName == permissionName && p.Level == level, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await dbContext.SaveChangesAsync(ct);
}
