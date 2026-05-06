using BuildingBlocks.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;

namespace Modules.Projects.Domain.Entities;

public sealed class Project : Entity
{
    private Project() { }

    private Project(
        Guid id,
        string name,
        string abbreviation,
        ProjectPrivacy privacy,
        IProjectOwner owner,
        OwnerType type,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Abbreviation = abbreviation;
        Privacy = privacy;
        CreatedAt = createdAt;
        Owner = owner;
        OwnerType = type;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Abbreviation { get; }
    public ProjectPrivacy Privacy { get; }
    public DateTime CreatedAt { get; }
    public IProjectOwner Owner { get; }
    public OwnerType OwnerType { get; }

    public static Project Create(
        string name,
        string abbreviation,
        ProjectPrivacy privacy,
        IProjectOwner owner,
        OwnerType type) =>
        new(Guid.NewGuid(), name, abbreviation, privacy, owner, type, DateTime.UtcNow);
}
