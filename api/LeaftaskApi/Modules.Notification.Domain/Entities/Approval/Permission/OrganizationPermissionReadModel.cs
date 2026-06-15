using BuildingBlocks.Domain.Entities;

namespace Modules.Notification.Domain.Entities.Approval.Permission;

public sealed class OrganizationPermissionReadModel : Entity
{
    private OrganizationPermissionReadModel() { }

    public OrganizationPermissionReadModel(Guid id, Guid userId, Guid organizationId, string permissionName, int level)
    {
        Id = id;
        UserId = userId;
        OrganizationId = organizationId;
        PermissionName = permissionName;
        Level = level;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public int Level { get; set; }
}
