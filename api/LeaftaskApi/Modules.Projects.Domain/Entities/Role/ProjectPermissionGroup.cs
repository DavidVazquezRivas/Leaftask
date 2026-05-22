using BuildingBlocks.Domain.Entities;

namespace Modules.Projects.Domain.Entities.Role;

public sealed class ProjectPermissionGroup : Entity
{
    private ProjectPermissionGroup() { }

    public ProjectPermissionGroup(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}
