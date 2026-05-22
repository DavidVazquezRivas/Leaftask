using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Fields.GetProjectCustomFields;

public sealed class GetProjectCustomFieldsQueryHandler(
    IProjectRepository projectRepository,
    IOrganizationPermissionChecker organizationPermissionChecker,
    IUserContext userContext,
    IGetProjectCustomFieldsQueryService queryService)
    : IQueryHandler<GetProjectCustomFieldsQuery, Result<IReadOnlyList<CustomFieldDto>>>
{
    public async Task<Result<IReadOnlyList<CustomFieldDto>>> Handle(
        GetProjectCustomFieldsQuery query,
        CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Failure<IReadOnlyList<CustomFieldDto>>(ProjectErrors.ProjectNotFound);
        }

        bool canAccess = await CanAccessAsync(project, userContext.UserId, cancellationToken);
        if (!canAccess)
        {
            return Result.Failure<IReadOnlyList<CustomFieldDto>>(ProjectErrors.AccessDenied);
        }

        IReadOnlyList<CustomFieldDto> fields = await queryService.GetCustomFieldsAsync(query.ProjectId, cancellationToken);
        return Result.Success(fields);
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
