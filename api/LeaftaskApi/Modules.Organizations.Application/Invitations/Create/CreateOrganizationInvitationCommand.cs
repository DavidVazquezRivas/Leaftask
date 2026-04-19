using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Authorization;

namespace Modules.Organizations.Application.Invitations.Create;

[RequireOrganizationPermission("Invite Members")]
public sealed record CreateOrganizationInvitationCommand(
    Guid OrganizationId,
    Guid UserId,
    Guid RoleId)
    : ICommand<Result<OrganizationInvitationResponse>>, IOrganizationPermissionRequest;
