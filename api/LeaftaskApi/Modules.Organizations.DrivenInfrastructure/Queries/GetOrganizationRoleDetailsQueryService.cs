using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Application.Roles.Create;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Queries;

public sealed class GetOrganizationRoleDetailsQueryService(OrganizationDbContext dbContext)
    : IGetOrganizationRoleDetailsQueryService
{
    public async Task<OrganizationRoleResponse?> GetOrganizationRoleAsync(
        Guid organizationId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        int totalMembers = await dbContext.OrganizationInvitations
            .AsNoTracking()
            .CountAsync(invitation => invitation.OrganizationId == organizationId && invitation.Status == InvitationStatus.Accepted, cancellationToken);

        OrganizationRoleResponse? role = await dbContext.OrganizationRoles
            .AsNoTracking()
            .Where(role => role.OrganizationId == organizationId && role.Id == roleId)
            .Select(role => new OrganizationRoleResponse(
                role.Id,
                role.Name,
                totalMembers,
                Array.Empty<CreateOrganizationRolePermissionResponse>()))
            .FirstOrDefaultAsync(cancellationToken);

        if (role is null)
        {
            return null;
        }

        IReadOnlyList<CreateOrganizationRolePermissionResponse> permissions = await dbContext.OrganizationRolePermissions
            .AsNoTracking()
            .Where(rolePermission => rolePermission.OrganizationRoleId == roleId && rolePermission.Level != PermissionLevel.None)
            .OrderBy(rolePermission => rolePermission.OrganizationPermissionId)
            .Select(rolePermission => new CreateOrganizationRolePermissionResponse(
                rolePermission.OrganizationPermissionId,
                rolePermission.Level))
            .ToListAsync(cancellationToken);

        return role with { Permissions = permissions };
    }
}
