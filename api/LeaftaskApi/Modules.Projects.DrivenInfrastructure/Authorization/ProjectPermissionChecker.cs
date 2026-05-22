using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.DrivenInfrastructure.Authorization;

public sealed class ProjectPermissionChecker(
    IProjectRepository projectRepository,
    IOrganizationPermissionChecker organizationPermissionChecker)
    : IProjectPermissionChecker
{
    public async Task<ProjectPermissionCheckStatus> CheckAsync(
        Guid projectId,
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default)
    {
        Project? project = await projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project is null)
        {
            return ProjectPermissionCheckStatus.ProjectNotFound;
        }

        if (project.OwnerType == OwnerType.Organization)
        {
            bool isMember = await organizationPermissionChecker.IsMemberAsync(project.OwnerId, userId, cancellationToken);
            return isMember ? ProjectPermissionCheckStatus.Full : ProjectPermissionCheckStatus.Denied;
        }

        return project.OwnerId == userId
            ? ProjectPermissionCheckStatus.Full
            : ProjectPermissionCheckStatus.Denied;
    }
}
