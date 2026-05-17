using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.WorkItems.Application.Configuration.GetWorkItemStatuses;
using Modules.WorkItems.Application.Configuration.GetWorkItemTypes;

namespace Modules.WorkItems.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/workitems")]
public sealed class WorkItemConfigurationController : ApiBaseController
{
    [HttpGet("types")]
    public async Task<IActionResult> GetWorkItemTypes(CancellationToken cancellationToken) =>
        HandleResult(await Sender.Send(new GetWorkItemTypesQuery(), cancellationToken));

    [HttpGet("statuses")]
    public async Task<IActionResult> GetWorkItemStatuses(CancellationToken cancellationToken) =>
        HandleResult(await Sender.Send(new GetWorkItemStatusesQuery(), cancellationToken));
}
