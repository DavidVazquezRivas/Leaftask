using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Integration;
using Modules.Projects.Application.Authorization;
using ProjectPermissionCheckStatus = Modules.Projects.Application.Authorization.OrganizationPermissionCheckStatus;
using OrganizationPermissionCheckStatus = Modules.Organizations.Integration.OrganizationPermissionCheckStatus;

namespace Modules.Projects.DrivenInfrastructure.Authorization;

public sealed class OrganizationPermissionChecker(
    IOrganizationPermissionService organizationPermissionService) : IOrganizationPermissionChecker
{
    public Task<bool> IsMemberAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        organizationPermissionService.IsOrganizationMemberAsync(organizationId, userId, cancellationToken);

    public async Task<ProjectPermissionCheckStatus> CheckAsync(
        Guid organizationId,
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default)
    {
        OrganizationPermissionCheckStatus status =
            await organizationPermissionService.CheckPermissionAsync(
                organizationId,
                userId,
                permissionName,
                cancellationToken);

        return status switch
        {
            OrganizationPermissionCheckStatus.Full => ProjectPermissionCheckStatus.Full,
            OrganizationPermissionCheckStatus.Supervised => ProjectPermissionCheckStatus.Supervised,
            OrganizationPermissionCheckStatus.Denied => ProjectPermissionCheckStatus.Denied,
            OrganizationPermissionCheckStatus.MembershipRequired => ProjectPermissionCheckStatus.MembershipRequired,
            OrganizationPermissionCheckStatus.PermissionNotFound => ProjectPermissionCheckStatus.PermissionNotFound,
            OrganizationPermissionCheckStatus.OrganizationNotFound => ProjectPermissionCheckStatus.OrganizationNotFound,
            _ => ProjectPermissionCheckStatus.Denied
        };
    }
}
