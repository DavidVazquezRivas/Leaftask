using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Permissions.GetRoles;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Permissions.PatchRole;

public sealed class PatchProjectRoleCommandHandler(
    IProjectRoleRepository roleRepository)
    : ICommandHandler<PatchProjectRoleCommand, Result<ProjectRoleDto>>
{
    public async Task<Result<ProjectRoleDto>> Handle(PatchProjectRoleCommand command, CancellationToken cancellationToken)
    {
        ProjectRole? role = await roleRepository.GetByIdTrackedAsync(command.ProjectId, command.RoleId, cancellationToken);
        if (role is null)
        {
            return Result.Failure<ProjectRoleDto>(ProjectErrors.RoleNotFound);
        }

        if (role.IsOwnerRole && command.Permissions is not null)
        {
            return Result.Failure<ProjectRoleDto>(ProjectErrors.OwnerRoleCannotBeModified);
        }

        if (command.Name is not null && command.Name != role.Name)
        {
            bool nameExists = await roleRepository.ExistsByNameAsync(
                command.ProjectId, command.Name, command.RoleId, cancellationToken);

            if (nameExists)
            {
                return Result.Failure<ProjectRoleDto>(ProjectErrors.DuplicatedRoleName);
            }

            role.Update(command.Name);
        }

        List<ProjectRolePermission> currentPermissions = await roleRepository.GetRolePermissionsTrackedAsync(role.Id, cancellationToken);
        List<ProjectRolePermission> finalPermissions;

        if (command.Permissions is not null)
        {
            List<(ProjectPermission Permission, PermissionLevel Level)> resolvedPermissions = [];
            foreach (PatchProjectRolePermissionInput input in command.Permissions)
            {
                ProjectPermission? permission = await roleRepository.GetPermissionByIdAsync(input.PermissionId, cancellationToken);
                if (permission is null)
                {
                    return Result.Failure<ProjectRoleDto>(ProjectErrors.PermissionNotFound);
                }

                resolvedPermissions.Add((permission, input.Level));
            }

            roleRepository.RemovePermissions(currentPermissions);

            finalPermissions = resolvedPermissions
                .Select(p => new ProjectRolePermission(Guid.NewGuid(), p.Level, p.Permission, role))
                .ToList();

            await roleRepository.AddPermissionsAsync(finalPermissions, cancellationToken);
        }
        else
        {
            finalPermissions = currentPermissions;
        }

        await roleRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(ToDto(role, finalPermissions));
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
