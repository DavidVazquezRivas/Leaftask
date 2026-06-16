namespace BuildingBlocks.Application.Authorization;

public enum ProjectPermissionCheckStatus
{
    Full = 0,
    Supervised = 1,
    Denied = 2,
    ProjectNotFound = 3
}
