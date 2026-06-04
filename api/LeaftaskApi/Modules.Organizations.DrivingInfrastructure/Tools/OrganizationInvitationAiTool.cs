using System.ComponentModel;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.Organizations.Application.Invitations;
using Modules.Organizations.Application.Invitations.GetPending;

namespace Modules.Organizations.DrivingInfrastructure.Tools;

public class OrganizationInvitationAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetPendingOrganizationInvitations")]
    [Description(
        "Retrieves a list of all pending user invitations for a specific organization. Useful to check who hasn't accepted their invite yet.")]
    public async Task<string> GetPendingInvitationsAsync(
        [Description("The unique identifier (GUID) of the organization.")]
        Guid orgId,
        CancellationToken cancellationToken = default)
    {
        GetPendingOrganizationInvitationsQuery query = new(orgId);

        Result<IReadOnlyList<OrganizationInvitationResponse>> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetPendingInvitationsAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }
}
