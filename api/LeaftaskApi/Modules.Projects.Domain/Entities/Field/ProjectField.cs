using BuildingBlocks.Domain.Entities;

namespace Modules.Projects.Domain.Entities.Field;

public sealed class ProjectField : Entity
{
    private ProjectField() { }

    public ProjectField(Guid id, string name, bool @default, bool optional, Field field, Project project)
    {
        Id = id;
        Name = name;
        Default = @default;
        Optional = optional;
        Field = field;
        Project = project;
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public bool Default { get; }
    public bool Optional { get; private set; }
    public Field Field { get; }
    public Project Project { get; }

    public void Update(string name, bool optional)
    {
        Name = name;
        Optional = optional;
    }
}
