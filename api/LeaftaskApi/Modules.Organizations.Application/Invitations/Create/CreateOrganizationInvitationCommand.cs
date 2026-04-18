using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Invitations.Create;

public sealed record CreateOrganizationInvitationCommand(
    Guid OrganizationId,
    Guid UserId,
    Guid RoleId)
    : ICommand<Result<OrganizationInvitationResponse>>;
