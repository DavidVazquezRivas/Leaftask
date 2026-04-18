using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Mvc;
using Modules.Organizations.Application.Members.Delete;
using Modules.Organizations.Application.Members.GetDistribution;
using Modules.Organizations.Application.Members.GetMembers;
using Modules.Organizations.Application.Members.UpdateRole;

namespace Modules.Organizations.DrivingInfrastructure.Controllers;

[Route("api/v1/organizations")]
public class OrganizationMembersController : ApiBaseController
{
    [HttpDelete("{orgId:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> DeleteMember(
        [FromRoute] Guid orgId,
        [FromRoute] Guid memberId,
        CancellationToken cancellationToken = default)
    {
        DeleteOrganizationMemberCommand command = new(orgId, memberId);

        Result result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPatch("{orgId:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> UpdateMemberRole(
        [FromRoute] Guid orgId,
        [FromRoute] Guid memberId,
        [FromBody] UpdateOrganizationMemberRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        UpdateOrganizationMemberRoleCommand command = new(orgId, memberId, request.RoleId);

        Result result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{orgId:guid}/members")]
    public async Task<IActionResult> GetMembers(
        [FromRoute] Guid orgId,
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] IReadOnlyCollection<string>? sort = null,
        CancellationToken cancellationToken = default)
    {
        GetOrganizationMembersQuery query = new()
        {
            OrganizationId = orgId,
            Limit = limit,
            Cursor = cursor,
            Sort = sort ?? []
        };

        Result<PaginatedResult<OrganizationMemberDto>> result = await Sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        IReadOnlyList<SortMeta>? sortMeta = SortMetaParser.Parse(sort);
        PaginationMeta paginationMeta = new()
        {
            Limit = limit,
            NextCursor = result.Value.NextCursor,
            HasMore = result.Value.HasMore
        };

        return StatusCode(200, BuildSuccessResponse(result.Value.Items, sortMeta, paginationMeta));
    }

    [HttpGet("{orgId:guid}/members/distribution")]
    public async Task<IActionResult> GetDistribution(
        [FromRoute] Guid orgId,
        CancellationToken cancellationToken = default)
    {
        GetOrganizationMembersDistributionQuery query = new(orgId);

        Result<IReadOnlyList<OrganizationMemberDistributionDto>> result = await Sender.Send(query, cancellationToken);

        return HandleResult(result);
    }
}
