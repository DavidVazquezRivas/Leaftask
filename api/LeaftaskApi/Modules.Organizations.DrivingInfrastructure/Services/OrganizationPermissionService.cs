using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.Integration;

namespace Modules.Organizations.DrivingInfrastructure.Services;

public sealed class OrganizationPermissionService(
    IOrganizationRepository organizationRepository,
    IOrganizationPermissionRepository organizationPermissionRepository) : IOrganizationPermissionService
{
    public async Task<bool> IsOrganizationMemberAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            return false;
        }

        return organization.Invitations.Any(inv =>
            inv.UserId == userId && inv.Status == InvitationStatus.Accepted);
    }

    public async Task<OrganizationPermissionCheckStatus> CheckPermissionAsync(
        Guid organizationId,
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            return OrganizationPermissionCheckStatus.OrganizationNotFound;
        }

        IReadOnlyCollection<OrganizationPermission> availablePermissions =
            await organizationPermissionRepository.GetAllAsync(cancellationToken);

        OrganizationPermission? requiredPermission = availablePermissions.FirstOrDefault(permission =>
            permission.Name.Equals(permissionName, StringComparison.OrdinalIgnoreCase));

        if (requiredPermission is null)
        {
            return OrganizationPermissionCheckStatus.PermissionNotFound;
        }

        OrganizationInvitation? invitation = organization.Invitations.FirstOrDefault(inv =>
            inv.UserId == userId && inv.Status == InvitationStatus.Accepted);

        if (invitation is null)
        {
            return OrganizationPermissionCheckStatus.MembershipRequired;
        }

        OrganizationRole? role = organization.Roles.FirstOrDefault(role => role.Id == invitation.OrganizationRoleId);
        OrganizationRolePermission? rolePermission = role?.Permissions.FirstOrDefault(permission =>
            permission.OrganizationPermissionId == requiredPermission.Id);

        return rolePermission?.Level switch
        {
            PermissionLevel.Full => OrganizationPermissionCheckStatus.Full,
            PermissionLevel.Supervised => OrganizationPermissionCheckStatus.Supervised,
            _ => OrganizationPermissionCheckStatus.Denied
        };
    }
}
