using System.ComponentModel;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.Organizations.Application.Members.GetDistribution;
using Modules.Organizations.Application.Members.GetMembers;

namespace Modules.Organizations.DrivingInfrastructure.Tools;

public class OrganizationMembersAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetOrganizationMembers")]
    [Description(
        "Retrieves a list of members belonging to a specific organization. Useful for finding who is part of the team or checking a member's current role.")]
    public async Task<string> GetMembersAsync(
        [Description("The unique identifier (GUID) of the organization.")]
        Guid orgId,
        [Description("Max results. Keep it low to avoid context overload. Default is 10.")]
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        GetOrganizationMembersQuery query = new()
        {
            OrganizationId = orgId,
            Limit = limit,
            Cursor = null,
            Sort = []
        };

        Result<PaginatedResult<OrganizationMemberDto>> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetMembersAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value.Items);
    }

    [KernelFunction("GetOrganizationMembersDistribution")]
    [Description(
        "Retrieves the distribution of members within an organization, typically grouped by their assigned roles. Useful for analyzing team composition and structure.")]
    public async Task<string> GetDistributionAsync(
        [Description("The unique identifier (GUID) of the organization.")]
        Guid orgId,
        CancellationToken cancellationToken = default)
    {
        GetOrganizationMembersDistributionQuery query = new(orgId);

        Result<IReadOnlyList<OrganizationMemberDistributionDto>> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetDistributionAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }
}
