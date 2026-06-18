using BuildingBlocks.Domain.Entities;

namespace Modules.Notification.Domain.Entities.Approval.Permission;

public sealed class ProjectPermissionReadModel : Entity
{
    private ProjectPermissionReadModel() { }

    public ProjectPermissionReadModel(Guid id, Guid userId, Guid projectId, string permissionName, int level)
    {
        Id = id;
        UserId = userId;
        ProjectId = projectId;
        PermissionName = permissionName;
        Level = level;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public int Level { get; set; }
}
