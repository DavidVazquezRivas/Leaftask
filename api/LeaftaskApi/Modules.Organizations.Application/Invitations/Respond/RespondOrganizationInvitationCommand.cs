using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Invitations.Respond;

public sealed record RespondOrganizationInvitationCommand(
    Guid OrganizationId,
    Guid InvitationId,
    string Status)
    : ICommand<Result<Modules.Organizations.Application.Invitations.OrganizationInvitationResponse>>;
