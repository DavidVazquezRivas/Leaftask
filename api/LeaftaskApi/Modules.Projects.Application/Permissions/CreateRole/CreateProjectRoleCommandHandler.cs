using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Permissions.GetRoles;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Permissions.CreateRole;

public sealed class CreateProjectRoleCommandHandler(
    IProjectRepository projectRepository,
    IProjectRoleRepository roleRepository)
    : ICommandHandler<CreateProjectRoleCommand, Result<ProjectRoleDto>>
{
    public async Task<Result<ProjectRoleDto>> Handle(CreateProjectRoleCommand command, CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetByIdTrackedAsync(command.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Failure<ProjectRoleDto>(ProjectErrors.ProjectNotFound);
        }

        bool nameExists = await roleRepository.ExistsByNameAsync(command.ProjectId, command.Name, cancellationToken: cancellationToken);
        if (nameExists)
        {
            return Result.Failure<ProjectRoleDto>(ProjectErrors.DuplicatedRoleName);
        }

        List<(ProjectPermission Permission, PermissionLevel Level)> resolvedPermissions = [];
        foreach (CreateProjectRolePermissionInput input in command.Permissions)
        {
            ProjectPermission? permission = await roleRepository.GetPermissionByIdAsync(input.PermissionId, cancellationToken);
            if (permission is null)
            {
                return Result.Failure<ProjectRoleDto>(ProjectErrors.PermissionNotFound);
            }

            resolvedPermissions.Add((permission, input.Level));
        }

        ProjectRole role = new(Guid.NewGuid(), command.Name, project);
        await roleRepository.AddAsync(role, cancellationToken);

        List<ProjectRolePermission> rolePermissions = resolvedPermissions
            .Select(p => new ProjectRolePermission(Guid.NewGuid(), p.Level, p.Permission, role))
            .ToList();

        await roleRepository.AddPermissionsAsync(rolePermissions, cancellationToken);
        await roleRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(ToDto(role, rolePermissions));
    }

    private static ProjectRoleDto ToDto(ProjectRole role, List<ProjectRolePermission> permissions) =>
        new(
            role.Id,
            role.Name,
            0,
            permissions.Select(rp => new ProjectRolePermissionDto(
                rp.Permission.Id,
                rp.Permission.Name,
                rp.Permission.PermissionGroup.Name,
                rp.PermissionLevel)).ToList());
}
