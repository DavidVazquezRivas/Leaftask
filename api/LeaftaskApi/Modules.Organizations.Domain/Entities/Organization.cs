using BuildingBlocks.Domain.Entities;

namespace Modules.Organizations.Domain.Entities;

public sealed class Organization : Entity
{
    private readonly List<OrganizationInvitation> _invitations = [];

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
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Website { get; private set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public IReadOnlyCollection<OrganizationInvitation> Invitations => _invitations.AsReadOnly();

    public void Update(string? name = null, string? description = null, string? website = null)
    {
        if (name is not null)
        {
            Name = name;
        }

        if (description is not null)
        {
            Description = description;
        }

        if (website is not null)
        {
            Website = website;
        }
    }

    public static Organization Create(string name, string description, string website, Guid creatorUserId)
    {
        Organization organization = new(Guid.NewGuid(), name, description, website, DateTime.UtcNow);

        OrganizationInvitation invitation = OrganizationInvitation.Create(organization.Id, creatorUserId);
        invitation.Accept();

        organization._invitations.Add(invitation);

        return organization;
    }
}
