using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Mvc;
using Modules.Projects.Application.Management.Create;
using Modules.Projects.Application.Management.GetMyProjects;
using Modules.Projects.Application.Management.GetOrganizationProjects;
using Modules.Projects.Application.Management.Delete;
using Modules.Projects.Application.Management.GetProject;
using Modules.Projects.Application.Management.Patch;
using Modules.Projects.DrivingInfrastructure.Models.Requests;

namespace Modules.Projects.DrivingInfrastructure.Controllers;

[Route("api/v1/projects")]
public class ProjectManagementController : ApiBaseController
{
    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProject(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        Result<ProjectResponse> result = await Sender.Send(new GetProjectQuery(projectId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{projectId}")]
    public async Task<IActionResult> PatchProject(
        Guid projectId,
        [FromBody] PatchProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        PatchProjectCommand command = new(projectId, request.Name, request.Abbreviation, request.PrivacyLevel);
        Result<ProjectResponse> result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        Result result = await Sender.Send(new DeleteProjectCommand(projectId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("me")]
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

    [HttpGet("organization/{organizationId}")]
    public async Task<IActionResult> GetOrganizationProjects(
        Guid organizationId,
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] IReadOnlyCollection<string>? sort = null,
        CancellationToken cancellationToken = default)
    {
        GetOrganizationProjectsQuery query = new()
        {
            OrganizationId = organizationId,
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
            request.PrivacyLevel,
            request.OrganizationId);

        Result<ProjectResponse> result = await Sender.Send(command, cancellationToken);

        return HandleResult(result, 201);
    }
}
