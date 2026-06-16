namespace BuildingBlocks.Application.Authorization;

public interface IProjectPermissionRequest
{
    Guid ProjectId { get; }
}
