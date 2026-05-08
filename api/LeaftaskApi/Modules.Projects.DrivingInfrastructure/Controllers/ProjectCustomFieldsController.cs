using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Projects.Application.Fields.CreateCustomField;
using Modules.Projects.Application.Fields.DeleteCustomField;
using Modules.Projects.Application.Fields.GetFieldTypes;
using Modules.Projects.Application.Fields.GetProjectCustomFields;
using Modules.Projects.Application.Fields.PatchCustomField;
using Modules.Projects.DrivingInfrastructure.Models.Requests;

namespace Modules.Projects.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/projects")]
public sealed class ProjectCustomFieldsController : ApiBaseController
{
    [HttpGet("field-types")]
    public async Task<IActionResult> GetFieldTypes(CancellationToken cancellationToken) =>
        HandleResult(await Sender.Send(new GetFieldTypesQuery(), cancellationToken));

    [HttpGet("{projectId:guid}/fields")]
    public async Task<IActionResult> GetCustomFields(Guid projectId, CancellationToken cancellationToken) =>
        HandleResult(await Sender.Send(new GetProjectCustomFieldsQuery(projectId), cancellationToken));

    [HttpPost("{projectId:guid}/fields")]
    public async Task<IActionResult> CreateCustomField(
        Guid projectId,
        [FromBody] CreateCustomFieldRequest request,
        CancellationToken cancellationToken)
    {
        Result<CustomFieldDto> result = await Sender.Send(
            new CreateCustomFieldCommand(
                projectId, request.Name, request.Type, request.Options, request.Required, request.AppliesTo),
            cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return StatusCode(201, result.Value);
    }

    [HttpPatch("{projectId:guid}/fields/{fieldId:guid}")]
    public async Task<IActionResult> PatchCustomField(
        Guid projectId,
        Guid fieldId,
        [FromBody] PatchCustomFieldRequest request,
        CancellationToken cancellationToken) =>
        HandleResult(await Sender.Send(
            new PatchCustomFieldCommand(
                projectId, fieldId, request.Name, request.Type, request.Options, request.Required, request.AppliesTo),
            cancellationToken));

    [HttpDelete("{projectId:guid}/fields/{fieldId:guid}")]
    public async Task<IActionResult> DeleteCustomField(
        Guid projectId,
        Guid fieldId,
        CancellationToken cancellationToken)
    {
        Result result = await Sender.Send(new DeleteCustomFieldCommand(projectId, fieldId), cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }
}
