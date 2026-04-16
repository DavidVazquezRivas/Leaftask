using BuildingBlocks.Domain.Entities;

namespace Modules.Organizations.Domain.Entities;

public sealed class Organization : Entity
{
    private Organization() { }

    private Organization(Guid id, string name, string description, string website, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Description = description;
        Website = website;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string Website { get; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public static Organization Create(string name, string description, string website)
    {
        return new Organization(Guid.NewGuid(), name, description, website, DateTime.UtcNow);
    }
}
