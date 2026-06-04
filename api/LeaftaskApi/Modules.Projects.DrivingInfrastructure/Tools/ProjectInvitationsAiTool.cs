using System.ComponentModel;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.Projects.Application.Invitations.GetPending;
using Modules.Projects.Application.Invitations.Invite;
using Modules.Projects.Application.Invitations.UpdateStatus;

namespace Modules.Projects.DrivingInfrastructure.Tools;

public class ProjectInvitationsAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetPendingProjectInvitations")]
    [Description(
        "Retrieves a list of pending invitations for a specific project. Useful to see who has been invited but hasn't responded yet, and to find specific Invitation IDs.")]
    public async Task<string> GetPendingInvitationsAsync(
        [Description(
            "The unique identifier (GUID) of the project. If you only have the project name, resolve it first using 'GetOrganizationProjects'.")]
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        Result<IReadOnlyList<ProjectInvitationDto>> result =
            await sender.Send(new GetPendingProjectInvitationsQuery(projectId), cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetPendingInvitationsAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("InviteProjectMember")]
    [Description("Invites a user to join a project and assigns them a specific role.")]
    public async Task<string> InviteMemberAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description(
            "The unique identifier (GUID) of the USER to invite. Do NOT confuse this with a Member ID. You MUST use 'SearchUsers' first to find the correct User ID if you only have a name or email.")]
        Guid userId,
        [Description(
            "The unique identifier (GUID) of the role to assign. Resolve semantic names to a GUID using 'GetProjectRoles' before calling this tool.")]
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        InviteProjectMemberCommand command = new(projectId, userId, roleId);

        Result<Guid> result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(InviteMemberAsync), result.Error.Description);
        }

        return formatter.FormatMessage($"User invited successfully. The new Invitation ID is: {result.Value}");
    }

    [KernelFunction("UpdateProjectInvitationStatus")]
    [Description("Updates the status of a pending project invitation (e.g., accepting, rejecting, or revoking it).")]
    public async Task<string> UpdateInvitationStatusAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description(
            "The unique identifier (GUID) of the invitation. Find this ID first using 'GetPendingProjectInvitations'.")]
        Guid invitationId,
        [Description("The enum representation of the new status")]
        string status,
        CancellationToken cancellationToken = default)
    {
        UpdateProjectInvitationStatusCommand command = new(projectId, invitationId, status);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(UpdateInvitationStatusAsync), result.Error.Description);
        }

        return formatter.FormatMessage("Project invitation status updated successfully.");
    }
}
