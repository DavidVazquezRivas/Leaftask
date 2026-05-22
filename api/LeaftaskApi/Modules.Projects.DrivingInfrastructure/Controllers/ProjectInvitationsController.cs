using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Projects.Application.Invitations.GetPending;
using Modules.Projects.Application.Invitations.Invite;
using Modules.Projects.Application.Invitations.UpdateStatus;
using Modules.Projects.DrivingInfrastructure.Models.Requests;

namespace Modules.Projects.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/projects")]
public sealed class ProjectInvitationsController : ApiBaseController
{
    [HttpGet("{projectId:guid}/invitations")]
    public async Task<IActionResult> GetPendingInvitations(Guid projectId, CancellationToken cancellationToken) =>
        HandleResult(await Sender.Send(new GetPendingProjectInvitationsQuery(projectId), cancellationToken));

    [HttpPost("{projectId:guid}/invitations")]
    public async Task<IActionResult> InviteMember(
        Guid projectId,
        [FromBody] InviteProjectMemberRequest request,
        CancellationToken cancellationToken)
    {
        Result<Guid> result = await Sender.Send(
            new InviteProjectMemberCommand(projectId, request.UserId, request.RoleId), cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        Response.Headers["X-Resource-ID"] = result.Value.ToString();
        return NoContent();
    }

    [HttpPatch("{projectId:guid}/invitations/{invitationId:guid}")]
    public async Task<IActionResult> UpdateInvitationStatus(
        Guid projectId,
        Guid invitationId,
        [FromBody] UpdateProjectInvitationStatusRequest request,
        CancellationToken cancellationToken)
    {
        Result result = await Sender.Send(
            new UpdateProjectInvitationStatusCommand(projectId, invitationId, request.Status),
            cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        Response.Headers["X-Resource-ID"] = invitationId.ToString();
        return NoContent();
    }
}
