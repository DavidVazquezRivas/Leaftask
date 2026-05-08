using Microsoft.EntityFrameworkCore;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Repositories;

public sealed class ProjectRoleRepository(ProjectsDbContext dbContext) : IProjectRoleRepository
{
    public async Task<ProjectRole?> GetByIdTrackedAsync(Guid projectId, Guid roleId, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectRoles
            .FirstOrDefaultAsync(
                r => r.Id == roleId && EF.Property<Guid>(r, "project_id") == projectId,
                cancellationToken);

    public async Task<bool> ExistsByNameAsync(Guid projectId, string name, Guid? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<ProjectRole> query = dbContext.ProjectRoles
            .Where(r => EF.Property<Guid>(r, "project_id") == projectId && r.Name == name);

        if (excludeRoleId.HasValue)
        {
            query = query.Where(r => r.Id != excludeRoleId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<ProjectPermission?> GetPermissionByIdAsync(Guid permissionId, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectPermissions
            .Include(p => p.PermissionGroup)
            .FirstOrDefaultAsync(p => p.Id == permissionId, cancellationToken);

    public async Task<List<ProjectRolePermission>> GetRolePermissionsTrackedAsync(Guid roleId, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectRolePermissions
            .Include(rp => rp.Permission).ThenInclude(p => p.PermissionGroup)
            .Where(rp => EF.Property<Guid>(rp, "project_role_id") == roleId)
            .ToListAsync(cancellationToken);

    public async Task<List<ProjectPermission>> GetAllPermissionsAsync(CancellationToken cancellationToken = default) =>
        await dbContext.ProjectPermissions
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ProjectRole role, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectRoles.AddAsync(role, cancellationToken);

    public void Remove(ProjectRole role) =>
        dbContext.ProjectRoles.Remove(role);

    public void RemovePermissions(IEnumerable<ProjectRolePermission> permissions) =>
        dbContext.ProjectRolePermissions.RemoveRange(permissions);

    public async Task AddPermissionsAsync(IEnumerable<ProjectRolePermission> permissions, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectRolePermissions.AddRangeAsync(permissions, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
