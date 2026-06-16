using BuildingBlocks.Application.Commands;

namespace Modules.Notification.Application.Organizations.RemoveMemberPermissions;

public sealed record RemoveMemberOrganizationPermissionsCommand(
    Guid OrganizationId,
    Guid UserId) : ICommand;
