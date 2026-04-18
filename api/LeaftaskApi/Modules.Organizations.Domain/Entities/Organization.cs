using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Errors;

namespace Modules.Organizations.Domain.Entities;

public sealed class Organization : Entity
{
    private readonly List<OrganizationInvitation> _invitations = [];
    private readonly List<OrganizationRole> _roles = [];

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
    public IReadOnlyCollection<OrganizationRole> Roles => _roles.AsReadOnly();

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

    public OrganizationRole AddRole(string name, IEnumerable<OrganizationPermission>? permissions = null)
    {
        OrganizationRole role = new(name, Id, permissions);
        _roles.Add(role);
        return role;
    }

    public Result UpdateRole(Guid roleId, string name, IReadOnlyCollection<(Guid OrganizationPermissionId, PermissionLevel Level)>? permissions = null)
    {
        OrganizationRole? role = _roles.SingleOrDefault(role => role.Id == roleId);
        if (role is null)
        {
            return Result.Failure(OrganizationErrors.OrganizationRoleNotFound);
        }

        return role.Update(name, permissions);
    }

    public Result RemoveRole(Guid roleId)
    {
        OrganizationRole? role = _roles.SingleOrDefault(role => role.Id == roleId);
        if (role is null)
        {
            return Result.Failure(OrganizationErrors.OrganizationRoleNotFound);
        }

        _roles.Remove(role);
        return Result.Success();
    }

    public static Organization Create(
        string name,
        string description,
        string website,
        Guid creatorUserId,
        IEnumerable<OrganizationPermission>? permissions = null)
    {
        Organization organization = new(Guid.NewGuid(), name, description, website, DateTime.UtcNow);

        OrganizationRole ownerRole = organization.AddRole("Owner", permissions);
        if (permissions is not null)
        {
            foreach (OrganizationPermission permission in permissions)
            {
                ownerRole.SetPermissionLevel(permission.Id, PermissionLevel.Full);
            }
        }

        OrganizationInvitation invitation = OrganizationInvitation.Create(organization.Id, creatorUserId, ownerRole.Id);
        invitation.Accept();

        organization._invitations.Add(invitation);

        return organization;
    }
}
