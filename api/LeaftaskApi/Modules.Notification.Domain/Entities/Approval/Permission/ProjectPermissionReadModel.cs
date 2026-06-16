using BuildingBlocks.Domain.Entities;

namespace Modules.Notification.Domain.Entities.Approval.Permission;

public sealed class ProjectPermissionReadModel : Entity
{
    private ProjectPermissionReadModel() { }

    public ProjectPermissionReadModel(Guid id) => Id = id;
    public Guid Id { get; set; }
}
