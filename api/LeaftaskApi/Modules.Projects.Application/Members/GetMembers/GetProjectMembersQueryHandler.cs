using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Members.GetMembers;

public sealed class GetProjectMembersQueryHandler(
    IProjectRepository projectRepository,
    IOrganizationPermissionChecker organizationPermissionChecker,
    IUserContext userContext,
    IGetProjectMembersQueryService queryService)
    : IQueryHandler<GetProjectMembersQuery, Result<PaginatedResult<ProjectMemberDto>>>
{
    public async Task<Result<PaginatedResult<ProjectMemberDto>>> Handle(
        GetProjectMembersQuery query,
        CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Failure<PaginatedResult<ProjectMemberDto>>(ProjectErrors.ProjectNotFound);
        }

        bool canAccess = await CanAccessAsync(project, userContext.UserId, cancellationToken);
        if (!canAccess)
        {
            return Result.Failure<PaginatedResult<ProjectMemberDto>>(ProjectErrors.AccessDenied);
        }

        PaginatedResult<ProjectMemberDto> members = await queryService.GetMembersAsync(
            query.ProjectId,
            query.Limit,
            query.Cursor,
            query.Sort,
            cancellationToken);

        return Result.Success(members);
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
