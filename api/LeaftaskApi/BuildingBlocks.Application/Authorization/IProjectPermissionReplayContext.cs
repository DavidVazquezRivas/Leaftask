namespace BuildingBlocks.Application.Authorization;

public interface IProjectPermissionReplayContext
{
    bool IsReplaying { get; }
    void BeginReplay();
}

public sealed class ProjectPermissionReplayContext : IProjectPermissionReplayContext
{
    public bool IsReplaying { get; private set; }
    public void BeginReplay() => IsReplaying = true;
}
