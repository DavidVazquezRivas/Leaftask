using BuildingBlocks.Domain.Entities;

namespace Modules.Projects.Domain.Entities.Role;

public sealed class ProjectRolePermission : Entity
{
    private ProjectRolePermission() { }

    public ProjectRolePermission(Guid id, PermissionLevel permissionLevel, ProjectPermission permission,
        ProjectRole role)
    {
        Id = id;
        PermissionLevel = permissionLevel;
        Permission = permission;
        Role = role;
    }

    public Guid Id { get; }
    public PermissionLevel PermissionLevel { get; private set; }
    public ProjectPermission Permission { get; }
    public ProjectRole Role { get; }
}
