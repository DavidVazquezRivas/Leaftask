using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Mvc;
using Modules.Organizations.Application.Management;
using Modules.Organizations.Application.Management.Create;
using Modules.Organizations.Application.Management.Delete;
using Modules.Organizations.Application.Management.GetDetails;
using Modules.Organizations.Application.Management.GetMyOrganizations;
using Modules.Organizations.Application.Management.Patch;
using Modules.Organizations.DrivingInfrastructure.Models.Requests;

namespace Modules.Organizations.DrivingInfrastructure.Controllers;

[Route("api/v1/organizations")]
public class OrganizationManagementController : ApiBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        CreateOrganizationCommand command = new(request.Name, request.Description, request.Website);

        Result<BasicOrganizationResponse> result = await Sender.Send(command, cancellationToken);

        return HandleResult(result, 201);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchOrganization(
        [FromRoute] Guid id,
        [FromBody] PatchOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        PatchOrganizationCommand command = new(id, request.Name, request.Description, request.Website);

        Result<BasicOrganizationResponse> result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOrganization(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        DeleteOrganizationCommand command = new(id);

        Result result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrganizationDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        GetOrganizationDetailsQuery query = new(id);

        Result<BasicOrganizationResponse> result = await Sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrganizations(
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] IReadOnlyCollection<string>? sort = null,
        CancellationToken cancellationToken = default)
    {
        GetMyOrganizationsQuery query = new()
        {
            Limit = limit,
            Cursor = cursor,
            Sort = sort ?? []
        };

        Result<PaginatedResult<SimpleOrganizationDto>> result = await Sender.Send(query, cancellationToken);

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
}
