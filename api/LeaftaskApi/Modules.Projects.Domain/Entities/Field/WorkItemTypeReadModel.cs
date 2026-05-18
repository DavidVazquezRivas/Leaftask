namespace Modules.Projects.Domain.Entities.Field;

public sealed class WorkItemTypeReadModel
{
    private WorkItemTypeReadModel() { }

    public WorkItemTypeReadModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; private set; } = string.Empty;
}
