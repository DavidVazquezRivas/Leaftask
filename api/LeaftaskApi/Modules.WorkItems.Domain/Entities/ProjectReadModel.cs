using BuildingBlocks.Domain.Entities;

namespace Modules.WorkItems.Domain.Entities;

public sealed class ProjectReadModel : Entity
{
    private ProjectReadModel() { }

    public ProjectReadModel(Guid id, string abbreviation)
    {
        Id = id;
        Abbreviation = abbreviation;
    }

    public Guid Id { get; }
    public string Abbreviation { get; }
}
