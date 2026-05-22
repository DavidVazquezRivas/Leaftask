using Microsoft.EntityFrameworkCore;
using Modules.Projects.Application.Permissions.GetPermissions;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Queries;

public sealed class GetProjectPermissionsQueryService(ProjectsDbContext dbContext)
    : IGetProjectPermissionsQueryService
{
    public async Task<IReadOnlyList<ProjectPermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default) =>
        await dbContext.ProjectPermissions
            .AsNoTracking()
            .Include(permission => permission.PermissionGroup)
            .OrderBy(permission => permission.Name)
            .Select(permission => new ProjectPermissionDto(
                permission.Id,
                permission.Name,
                permission.Description,
                permission.PermissionGroup.Name))
            .ToListAsync(cancellationToken);
}
