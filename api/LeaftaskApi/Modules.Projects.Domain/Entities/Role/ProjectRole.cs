using BuildingBlocks.Domain.Entities;

namespace Modules.Projects.Domain.Entities.Role;

public sealed class ProjectRole : Entity
{
    private ProjectRole() { }

    public ProjectRole(Guid id, string name, Project project)
    {
        Id = id;
        Name = name;
        Project = project;
    }

    public Guid Id { get; }
    public string Name { get; }
    public Project Project { get; }
}
