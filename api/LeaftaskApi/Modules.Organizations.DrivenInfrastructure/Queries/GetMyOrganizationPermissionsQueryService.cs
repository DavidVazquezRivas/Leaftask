using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Application.Roles.GetMyPermissions;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Queries;

public sealed class GetMyOrganizationPermissionsQueryService(OrganizationDbContext dbContext)
    : IGetMyOrganizationPermissionsQueryService
{
    public async Task<IReadOnlyList<MyOrganizationPermissionDto>> GetMyPermissionsAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        Guid? roleId = await dbContext.OrganizationInvitations
            .AsNoTracking()
            .Where(invitation => invitation.OrganizationId == organizationId
                                 && invitation.UserId == userId
                                 && invitation.Status == InvitationStatus.Accepted)
            .Select(invitation => (Guid?)invitation.OrganizationRoleId)
            .SingleOrDefaultAsync(cancellationToken);

        if (roleId is null)
        {
            return [];
        }

        return await dbContext.OrganizationRolePermissions
            .AsNoTracking()
            .Where(rolePermission => rolePermission.OrganizationRoleId == roleId.Value)
            .Join(
                dbContext.OrganizationPermissions.AsNoTracking(),
                rolePermission => rolePermission.OrganizationPermissionId,
                permission => permission.Id,
                (rolePermission, permission) => new MyOrganizationPermissionDto(
                    permission.Id,
                    permission.Name,
                    (int)rolePermission.Level))
            .OrderBy(permission => permission.Name)
            .ToListAsync(cancellationToken);
    }
}
