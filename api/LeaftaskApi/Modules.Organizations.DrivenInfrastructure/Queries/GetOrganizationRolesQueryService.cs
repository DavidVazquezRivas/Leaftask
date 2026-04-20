using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Application.Roles.GetRoles;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Queries;

public sealed class GetOrganizationRolesQueryService(OrganizationDbContext dbContext)
    : IGetOrganizationRolesQueryService
{
    public async Task<IReadOnlyList<OrganizationRoleDto>> GetOrganizationRolesAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        List<OrganizationRoleDto> roles = await dbContext.OrganizationRoles
            .AsNoTracking()
            .Where(role => role.OrganizationId == organizationId)
            .OrderBy(role => role.Name)
            .Select(role => new OrganizationRoleDto(
                role.Id,
                role.Name,
                0,
                Array.Empty<OrganizationRolePermissionDto>()))
            .ToListAsync(cancellationToken);

        if (roles.Count == 0)
        {
            return roles;
        }

        Guid[] roleIds = roles.Select(role => role.Id).ToArray();

        Dictionary<Guid, int> membersByRole = await dbContext.OrganizationInvitations
            .AsNoTracking()
            .Where(invitation => invitation.OrganizationId == organizationId
                                 && invitation.Status == InvitationStatus.Accepted
                                 && roleIds.Contains(invitation.OrganizationRoleId))
            .GroupBy(invitation => invitation.OrganizationRoleId)
            .Select(group => new { RoleId = group.Key, TotalMembers = group.Count() })
            .ToDictionaryAsync(item => item.RoleId, item => item.TotalMembers, cancellationToken);

        List<(Guid RoleId, OrganizationRolePermissionDto Permission)> permissions = await dbContext.OrganizationRolePermissions
            .AsNoTracking()
            .Where(rolePermission => rolePermission.Level != PermissionLevel.None && roleIds.Contains(rolePermission.OrganizationRoleId))
            .OrderBy(rolePermission => rolePermission.OrganizationRoleId)
            .ThenBy(rolePermission => rolePermission.OrganizationPermissionId)
            .Select(rolePermission => new ValueTuple<Guid, OrganizationRolePermissionDto>(
                rolePermission.OrganizationRoleId,
                new OrganizationRolePermissionDto(
                    rolePermission.OrganizationPermissionId,
                    (int)rolePermission.Level)))
            .ToListAsync(cancellationToken);

        Dictionary<Guid, List<OrganizationRolePermissionDto>> permissionMap = permissions
            .GroupBy(item => item.RoleId)
            .ToDictionary(group => group.Key, group => group.Select(item => item.Permission).ToList());

        return roles
            .Select(role => role with
            {
                TotalMembers = membersByRole.TryGetValue(role.Id, out int totalMembers)
                    ? totalMembers
                    : 0,
                Permissions = permissionMap.TryGetValue(role.Id, out List<OrganizationRolePermissionDto>? rolePermissions)
                    ? rolePermissions
                    : []
            })
            .ToList();
    }
}
