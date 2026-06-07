using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.DrivenInfrastructure.Authorization;

public sealed class ProjectPermissionChecker(
    IProjectRepository projectRepository,
    IProjectMemberRepository memberRepository,
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

        // Direct project membership covers both regular users and agents
        bool isDirectMember = await memberRepository.ExistsByMemberIdAsync(projectId, userId, cancellationToken);
        if (isDirectMember)
            return ProjectPermissionCheckStatus.Full;

        // Project owner (personal projects)
        if (project.OwnerType == OwnerType.User && project.OwnerId == userId)
            return ProjectPermissionCheckStatus.Full;

        // Organization membership (org-owned projects)
        if (project.OwnerType == OwnerType.Organization)
        {
            bool isOrgMember = await organizationPermissionChecker.IsMemberAsync(
                project.OwnerId, userId, cancellationToken);
            return isOrgMember ? ProjectPermissionCheckStatus.Full : ProjectPermissionCheckStatus.Denied;
        }

        return ProjectPermissionCheckStatus.Denied;
    }
}
