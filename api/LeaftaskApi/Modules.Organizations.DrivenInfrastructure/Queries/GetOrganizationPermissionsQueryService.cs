using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Application.Roles.GetPermissions;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Queries;

public sealed class GetOrganizationPermissionsQueryService(OrganizationDbContext dbContext)
    : IGetOrganizationPermissionsQueryService
{
    public async Task<IReadOnlyList<OrganizationPermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default) =>
        await dbContext.OrganizationPermissions
            .AsNoTracking()
            .OrderBy(permission => permission.Name)
            .Select(permission => new OrganizationPermissionDto(
                permission.Id,
                permission.Name,
                permission.Description))
            .ToListAsync(cancellationToken);
}
