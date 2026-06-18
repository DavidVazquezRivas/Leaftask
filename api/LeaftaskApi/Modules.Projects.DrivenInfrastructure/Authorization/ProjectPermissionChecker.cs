using BuildingBlocks.Application.Authorization;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.DrivenInfrastructure.Authorization;

public sealed class ProjectPermissionChecker(
    IProjectRepository projectRepository,
    IProjectMemberRepository memberRepository,
    IProjectRoleRepository roleRepository,
    IOrganizationPermissionChecker organizationPermissionChecker)
    : IProjectPermissionChecker
{
    public async Task<ProjectPermissionCheckStatus> CheckAsync(
        Guid projectId,
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return ProjectPermissionCheckStatus.Denied;

        Project? project = await projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project is null)
            return ProjectPermissionCheckStatus.ProjectNotFound;

        if (project.OwnerType == OwnerType.User && project.OwnerId == userId)
            return ProjectPermissionCheckStatus.Full;

        ProjectMember? member = await memberRepository.GetByMemberIdTrackedAsync(projectId, userId, cancellationToken);
        if (member is not null)
        {
            if (member.Role.IsOwnerRole)
                return ProjectPermissionCheckStatus.Full;

            List<ProjectRolePermission> rolePermissions =
                await roleRepository.GetRolePermissionsTrackedAsync(member.Role.Id, cancellationToken);

            ProjectRolePermission? rolePermission = rolePermissions
                .FirstOrDefault(rp => rp.Permission.Name.Equals(permissionName, StringComparison.OrdinalIgnoreCase));

            if (rolePermission is null)
                return ProjectPermissionCheckStatus.Full;

            return rolePermission.PermissionLevel switch
            {
                PermissionLevel.None => ProjectPermissionCheckStatus.Denied,
                PermissionLevel.Supervised => ProjectPermissionCheckStatus.Supervised,
                _ => ProjectPermissionCheckStatus.Full
            };
        }

        if (project.OwnerType == OwnerType.Organization)
        {
            bool isOrgMember = await organizationPermissionChecker.IsMemberAsync(
                project.OwnerId, userId, cancellationToken);
            return isOrgMember ? ProjectPermissionCheckStatus.Full : ProjectPermissionCheckStatus.Denied;
        }

        return ProjectPermissionCheckStatus.Denied;
    }
}
