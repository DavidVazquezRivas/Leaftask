using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Permissions.DeleteRole;

public sealed class DeleteProjectRoleCommandHandler(
    IProjectRoleRepository roleRepository)
    : ICommandHandler<DeleteProjectRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectRoleCommand command, CancellationToken cancellationToken)
    {
        Domain.Entities.Role.ProjectRole? role = await roleRepository.GetByIdTrackedAsync(
            command.ProjectId, command.RoleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure(ProjectErrors.RoleNotFound);
        }

        if (role.IsOwnerRole)
        {
            return Result.Failure(ProjectErrors.OwnerRoleCannotBeDeleted);
        }

        roleRepository.Remove(role);
        await roleRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
