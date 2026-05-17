using BuildingBlocks.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Events;

namespace Modules.Projects.Domain.Entities;

public sealed class Project : Entity
{
    private Project() { }

    private Project(
        Guid id,
        string name,
        string abbreviation,
        ProjectPrivacy privacy,
        Guid ownerId,
        OwnerType type,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Abbreviation = abbreviation;
        Privacy = privacy;
        CreatedAt = createdAt;
        OwnerId = ownerId;
        OwnerType = type;
    }

    public Guid Id { get; }
    public string Name { get; private set; } = string.Empty;
    public string Abbreviation { get; private set; } = string.Empty;
    public ProjectPrivacy Privacy { get; private set; }
    public DateTime CreatedAt { get; }
    public Guid OwnerId { get; private set; }
    public OwnerType OwnerType { get; }
    public IProjectOwner Owner => new ProjectOwnerReference(OwnerId);

    public void Update(string? name, string? abbreviation, ProjectPrivacy? privacy)
    {
        if (name is not null) Name = name;
        if (abbreviation is not null) Abbreviation = abbreviation;
        if (privacy.HasValue) Privacy = privacy.Value;
    }

    public void Delete() => Raise(new ProjectDeletedDomainEvent(Id));

    public static Project Create(
        string name,
        string abbreviation,
        ProjectPrivacy privacy,
        IProjectOwner owner,
        OwnerType type)
    {
        Project project = new(Guid.NewGuid(), name, abbreviation, privacy, owner.Id, type, DateTime.UtcNow);
        project.Raise(new ProjectCreatedDomainEvent(project.Id, project.Abbreviation));
        return project;
    }
}
