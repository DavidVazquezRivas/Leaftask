using BuildingBlocks.Application.Commands;

namespace Modules.Notification.Application.Organizations.SyncMemberPermissions;

public sealed record OrganizationPermissionEntryDto(string PermissionName, int Level);

public sealed record SyncMemberOrganizationPermissionsCommand(
    Guid OrganizationId,
    Guid UserId,
    IReadOnlyCollection<OrganizationPermissionEntryDto> Permissions) : ICommand;
