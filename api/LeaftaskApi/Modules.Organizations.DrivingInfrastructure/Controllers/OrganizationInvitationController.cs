using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Mvc;
using Modules.Organizations.Application.Invitations;
using Modules.Organizations.Application.Invitations.Create;
using Modules.Organizations.Application.Invitations.GetPending;
using Modules.Organizations.Application.Invitations.Respond;
using Modules.Organizations.DrivingInfrastructure.Models.Requests;

namespace Modules.Organizations.DrivingInfrastructure.Controllers;

[Route("api/v1/organizations")]
public class OrganizationInvitationController : ApiBaseController
{
    [HttpPatch("{orgId:guid}/invitations/{invitationId:guid}")]
    public async Task<IActionResult> RespondInvitation(
        [FromRoute] Guid orgId,
        [FromRoute] Guid invitationId,
        [FromBody] RespondOrganizationInvitationRequest request,
        CancellationToken cancellationToken = default)
    {
        RespondOrganizationInvitationCommand command = new(orgId, invitationId, request.Status);

        Result<OrganizationInvitationResponse> result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{orgId:guid}/invitations")]
    public async Task<IActionResult> CreateInvitation(
        [FromRoute] Guid orgId,
        [FromBody] CreateOrganizationInvitationRequest request,
        CancellationToken cancellationToken = default)
    {
        CreateOrganizationInvitationCommand command = new(orgId, request.UserId, request.RoleId);

        Result<OrganizationInvitationResponse> result = await Sender.Send(command, cancellationToken);

        return HandleResult(result, 201);
    }

    [HttpGet("{orgId:guid}/invitations/pending")]
    public async Task<IActionResult> GetPendingInvitations(
        [FromRoute] Guid orgId,
        CancellationToken cancellationToken = default)
    {
        GetPendingOrganizationInvitationsQuery query = new(orgId);

        Result<IReadOnlyList<OrganizationInvitationResponse>> result = await Sender.Send(query, cancellationToken);

        return HandleResult(result);
    }
}
