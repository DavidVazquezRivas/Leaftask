using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Members.GetCount;

public sealed class GetProjectMemberCountQueryHandler(
    IProjectRepository projectRepository,
    IProjectMemberRepository projectMemberRepository,
    IOrganizationPermissionChecker organizationPermissionChecker,
    IUserContext userContext)
    : IQueryHandler<GetProjectMemberCountQuery, Result<ProjectMemberCountDto>>
{
    public async Task<Result<ProjectMemberCountDto>> Handle(
        GetProjectMemberCountQuery query,
        CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Failure<ProjectMemberCountDto>(ProjectErrors.ProjectNotFound);
        }

        bool canAccess = await CanAccessAsync(project, userContext.UserId, cancellationToken);
        if (!canAccess)
        {
            return Result.Failure<ProjectMemberCountDto>(ProjectErrors.AccessDenied);
        }

        (int people, int agents) = await projectMemberRepository.GetCountByProjectAsync(query.ProjectId, cancellationToken);

        return Result.Success(new ProjectMemberCountDto(people + agents, people, agents));
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
