using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.DrivenInfrastructure.Authorization;

public sealed class ProjectAccessChecker(
    IProjectMemberRepository memberRepository,
    IOrganizationPermissionChecker organizationPermissionChecker) : IProjectAccessChecker
{
    public async Task<bool> CanAccessAsync(
        Project project,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return false;

        // Direct project membership — covers regular users and agents
        if (await memberRepository.ExistsByMemberIdAsync(project.Id, userId, cancellationToken))
            return true;

        // Personal project owner
        if (project.OwnerType == OwnerType.User)
            return project.OwnerId == userId;

        // Organisation member for org-owned projects
        return await organizationPermissionChecker.IsMemberAsync(
            project.OwnerId, userId, cancellationToken);
    }
}
