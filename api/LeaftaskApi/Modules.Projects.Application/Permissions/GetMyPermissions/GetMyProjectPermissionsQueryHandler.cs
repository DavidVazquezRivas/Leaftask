using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Permissions.GetPermissions;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Permissions.GetMyPermissions;

public sealed class GetMyProjectPermissionsQueryHandler(
    IProjectRepository projectRepository,
    IProjectMemberRepository memberRepository,
    IProjectRoleRepository roleRepository,
    IOrganizationPermissionChecker organizationPermissionChecker,
    IGetProjectPermissionsQueryService allPermissionsService,
    IUserContext userContext)
    : IQueryHandler<GetMyProjectPermissionsQuery, Result<IReadOnlyList<ProjectPermissionLevelDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectPermissionLevelDto>>> Handle(
        GetMyProjectPermissionsQuery query,
        CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure<IReadOnlyList<ProjectPermissionLevelDto>>(ProjectErrors.ProjectNotFound);

        IReadOnlyList<ProjectPermissionDto> allPermissions =
            await allPermissionsService.GetPermissionsAsync(cancellationToken);

        Guid userId = userContext.UserId;

        if (project.OwnerType == OwnerType.User && project.OwnerId == userId)
            return Result.Success<IReadOnlyList<ProjectPermissionLevelDto>>(AllFull(allPermissions));

        ProjectMember? member = await memberRepository.GetByMemberIdTrackedAsync(
            query.ProjectId, userId, cancellationToken);

        if (member is not null)
        {
            if (member.Role.IsOwnerRole)
                return Result.Success<IReadOnlyList<ProjectPermissionLevelDto>>(AllFull(allPermissions));

            List<ProjectRolePermission> rolePermissions =
                await roleRepository.GetRolePermissionsTrackedAsync(member.Role.Id, cancellationToken);

            List<ProjectPermissionLevelDto> levels = allPermissions
                .Select(p =>
                {
                    ProjectRolePermission? rp = rolePermissions
                        .FirstOrDefault(x => x.Permission.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase));

                    string level = rp?.PermissionLevel switch
                    {
                        PermissionLevel.None => "none",
                        PermissionLevel.Supervised => "supervised",
                        _ => "full"
                    };

                    return new ProjectPermissionLevelDto(p.Name, level);
                })
                .ToList();

            return Result.Success<IReadOnlyList<ProjectPermissionLevelDto>>(levels);
        }

        if (project.OwnerType == OwnerType.Organization)
        {
            bool isOrgMember = await organizationPermissionChecker.IsMemberAsync(
                project.OwnerId, userId, cancellationToken);

            if (isOrgMember)
                return Result.Success<IReadOnlyList<ProjectPermissionLevelDto>>(AllFull(allPermissions));
        }

        return Result.Failure<IReadOnlyList<ProjectPermissionLevelDto>>(ProjectErrors.AccessDenied);
    }

    private static List<ProjectPermissionLevelDto> AllFull(IReadOnlyList<ProjectPermissionDto> permissions) =>
        permissions.Select(p => new ProjectPermissionLevelDto(p.Name, "full")).ToList();
}
