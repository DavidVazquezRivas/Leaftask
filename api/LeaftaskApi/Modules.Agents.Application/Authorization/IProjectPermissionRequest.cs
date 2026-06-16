namespace Modules.Agents.Application.Authorization;

public interface IProjectPermissionRequest
{
    Guid ProjectId { get; }
}
