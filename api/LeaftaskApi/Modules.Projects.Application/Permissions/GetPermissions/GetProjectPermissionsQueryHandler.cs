using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Permissions.GetPermissions;

public sealed class GetProjectPermissionsQueryHandler(
    IProjectRepository projectRepository,
    IOrganizationPermissionChecker organizationPermissionChecker,
    IUserContext userContext,
    IGetProjectPermissionsQueryService permissionsService)
    : IQueryHandler<GetProjectPermissionsQuery, Result<IReadOnlyList<ProjectPermissionDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectPermissionDto>>> Handle(
        GetProjectPermissionsQuery query,
        CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Failure<IReadOnlyList<ProjectPermissionDto>>(ProjectErrors.ProjectNotFound);
        }

        bool canAccess = await CanAccessAsync(project, userContext.UserId, cancellationToken);
        if (!canAccess)
        {
            return Result.Failure<IReadOnlyList<ProjectPermissionDto>>(ProjectErrors.AccessDenied);
        }

        IReadOnlyList<ProjectPermissionDto> permissions = await permissionsService.GetPermissionsAsync(cancellationToken);
        return Result.Success(permissions);
    }

    private async Task<bool> CanAccessAsync(Project project, Guid userId, CancellationToken cancellationToken)
    {
        if (project.OwnerType == OwnerType.Organization)
        {
            return await organizationPermissionChecker.IsMemberAsync(project.OwnerId, userId, cancellationToken);
        }

        return project.OwnerId == userId;
    }
}
