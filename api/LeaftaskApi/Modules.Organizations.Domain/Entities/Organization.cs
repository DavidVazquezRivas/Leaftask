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
    public string Name { get; }
    public string Description { get; }
    public string Website { get; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public IReadOnlyCollection<OrganizationInvitation> Invitations => _invitations.AsReadOnly();

    public static Organization Create(string name, string description, string website, Guid creatorUserId)
    {
        Organization organization = new(Guid.NewGuid(), name, description, website, DateTime.UtcNow);

        OrganizationInvitation invitation = OrganizationInvitation.Create(organization.Id, creatorUserId);
        invitation.Accept();

        organization._invitations.Add(invitation);

        return organization;
    }
}
