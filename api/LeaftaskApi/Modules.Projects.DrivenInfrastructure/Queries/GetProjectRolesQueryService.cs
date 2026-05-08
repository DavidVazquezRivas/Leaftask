using Microsoft.EntityFrameworkCore;
using Modules.Projects.Application.Permissions.GetRoles;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Queries;

public sealed class GetProjectRolesQueryService(ProjectsDbContext dbContext) : IGetProjectRolesQueryService
{
    public async Task<IReadOnlyList<ProjectRoleDto>> GetRolesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        List<ProjectRole> roles = await dbContext.ProjectRoles
            .AsNoTracking()
            .Where(r => EF.Property<Guid>(r, "project_id") == projectId)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        if (roles.Count == 0)
        {
            return [];
        }

        HashSet<Guid> roleIds = roles.Select(r => r.Id).ToHashSet();

        List<ProjectRolePermission> rolePermissions = await dbContext.ProjectRolePermissions
            .AsNoTracking()
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission).ThenInclude(p => p.PermissionGroup)
            .Where(rp => roleIds.Contains(EF.Property<Guid>(rp, "project_role_id")))
            .ToListAsync(cancellationToken);

        Dictionary<Guid, List<ProjectRolePermission>> permsByRoleId = rolePermissions
            .GroupBy(rp => rp.Role.Id)
            .ToDictionary(g => g.Key, g => g.ToList());

        return roles
            .Select(r =>
            {
                List<ProjectRolePermission> perms = permsByRoleId.TryGetValue(r.Id, out List<ProjectRolePermission>? p) ? p : [];
                return new ProjectRoleDto(
                    r.Id,
                    r.Name,
                    0,
                    perms.Select(rp => new ProjectRolePermissionDto(
                        rp.Permission.Id,
                        rp.Permission.Name,
                        rp.Permission.PermissionGroup.Name,
                        rp.PermissionLevel)).ToList());
            })
            .ToList();
    }
}
