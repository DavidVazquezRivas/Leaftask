using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Mvc;
using Modules.Projects.Application.Management.Create;
using Modules.Projects.Application.Management.GetMyProjects;
using Modules.Projects.Application.Management.GetOrganizationProjects;
using Modules.Projects.DrivingInfrastructure.Models.Requests;

namespace Modules.Projects.DrivingInfrastructure.Controllers;

[Route("api/v1/projects")]
public class ProjectManagementController : ApiBaseController
{
    [HttpGet("/me")]
    public async Task<IActionResult> GetMyProjects(
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] IReadOnlyCollection<string>? sort = null,
        CancellationToken cancellationToken = default)
    {
        GetMyProjectsQuery query = new()
        {
            Limit = limit,
            Cursor = cursor,
            Sort = sort ?? []
        };

        Result<PaginatedResult<SimpleProjectDto>> result = await Sender.Send(query, cancellationToken);

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

    [HttpGet("/organization/{organizationId}")]
    public async Task<IActionResult> GetOrganizationProjects(
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] IReadOnlyCollection<string>? sort = null,
        CancellationToken cancellationToken = default)
    {
        GetOrganizationProjectsQuery query = new()
        {
            Limit = limit,
            Cursor = cursor,
            Sort = sort ?? []
        };

        Result<PaginatedResult<SimpleProjectDto>> result = await Sender.Send(query, cancellationToken);

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

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        CreateProjectCommand command = new(
            request.Name,
            request.Abbreviation,
            request.PrivacyLevelId,
            request.OrganizationId);

        Result<ProjectResponse> result = await Sender.Send(command, cancellationToken);

        return HandleResult(result, 201);
    }
}
