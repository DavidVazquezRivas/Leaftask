namespace Modules.WorkItems.Application.Authorization;

public interface IProjectPermissionRequest
{
    Guid ProjectId { get; }
}
