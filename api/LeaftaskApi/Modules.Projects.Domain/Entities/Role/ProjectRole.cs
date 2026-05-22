using BuildingBlocks.Domain.Entities;

namespace Modules.Projects.Domain.Entities.Role;

public sealed class ProjectRole : Entity
{
    private ProjectRole() { }

    public ProjectRole(Guid id, string name, Project project, bool isOwnerRole = false)
    {
        Id = id;
        Name = name;
        Project = project;
        IsOwnerRole = isOwnerRole;
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public bool IsOwnerRole { get; }
    public Project Project { get; }

    public void Update(string name)
    {
        Name = name;
    }
}
