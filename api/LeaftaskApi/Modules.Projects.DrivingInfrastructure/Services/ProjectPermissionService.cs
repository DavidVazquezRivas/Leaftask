using BuildingBlocks.Application.Authorization;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Events;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.DrivingInfrastructure.Services;

public sealed class ProjectPermissionService(
    IProjectPermissionChecker permissionChecker,
    IProjectRepository projectRepository)
    : IProjectPermissionService
{
    public async Task<ProjectPermissionCheckStatus> CheckPermissionAsync(
        Guid projectId,
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default) =>
        await permissionChecker.CheckAsync(projectId, userId, permissionName, cancellationToken);

    public async Task RequestApprovalAsync(
        Guid projectId,
        Guid userId,
        string permissionName,
        string actionName,
        string actionPayload,
        CancellationToken cancellationToken = default)
    {
        Domain.Entities.Project? project = await projectRepository.GetByIdTrackedAsync(projectId, cancellationToken);
        if (project is null)
            return;

        project.Raise(new ProjectPermissionActionRequestedDomainEvent(
            project.Id, userId, permissionName, actionName, actionPayload));

        await projectRepository.SaveChangesAsync(cancellationToken);
    }
}
