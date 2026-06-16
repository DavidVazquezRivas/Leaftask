using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Permissions.GetRoles;

public sealed class GetProjectRolesQueryHandler(
    IProjectRepository projectRepository,
    IProjectAccessChecker accessChecker,
    IUserContext userContext,
    IGetProjectRolesQueryService rolesService)
    : IQueryHandler<GetProjectRolesQuery, Result<IReadOnlyList<ProjectRoleDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectRoleDto>>> Handle(
        GetProjectRolesQuery query,
        CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure<IReadOnlyList<ProjectRoleDto>>(ProjectErrors.ProjectNotFound);

        if (!await accessChecker.CanAccessAsync(project, userContext.UserId, cancellationToken))
            return Result.Failure<IReadOnlyList<ProjectRoleDto>>(ProjectErrors.AccessDenied);

        IReadOnlyList<ProjectRoleDto> roles = await rolesService.GetRolesAsync(query.ProjectId, cancellationToken);
        return Result.Success(roles);
    }
}
