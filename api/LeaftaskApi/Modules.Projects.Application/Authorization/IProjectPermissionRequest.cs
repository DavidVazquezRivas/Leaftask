namespace Modules.Projects.Application.Authorization;

public interface IProjectPermissionRequest
{
    Guid ProjectId { get; }
}
