using BuildingBlocks.Application.Commands;

namespace Modules.Notification.Application.Projects.SyncMemberPermissions;

public sealed record SyncMemberProjectPermissionsCommand(
    Guid ProjectId,
    Guid UserId,
    IReadOnlyCollection<ProjectPermissionEntryDto> Permissions) : ICommand;

public sealed record ProjectPermissionEntryDto(string PermissionName, int Level);
