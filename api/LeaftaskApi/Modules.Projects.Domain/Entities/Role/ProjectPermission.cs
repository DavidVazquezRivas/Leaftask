using BuildingBlocks.Domain.Entities;

namespace Modules.Projects.Domain.Entities.Role;

public sealed class ProjectPermission : Entity
{
    private ProjectPermission() { }

    public ProjectPermission(Guid id, string name, string description, ProjectPermissionGroup permissionGroup)
    {
        Id = id;
        Name = name;
        Description = description;
        PermissionGroup = permissionGroup;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public ProjectPermissionGroup PermissionGroup { get; }
}
