using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Agents.Application.Agents;
using Modules.Agents.Application.Agents.Create;
using Modules.Agents.DrivingInfrastructure.Models.Requests;

namespace Modules.Agents.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/agents")]
public sealed class AgentManagementController : ApiBaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateAgent(
        [FromBody] CreateAgentRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<AgentDto> result = await Sender.Send(
            new CreateAgentCommand(
                request.ProjectId ?? Guid.Empty,
                request.Name,
                request.Instructions,
                Guid.TryParse(request.TemplateId, out Guid tid) ? tid : null,
                request.RoleId ?? Guid.Empty),
            cancellationToken);

        return HandleResult(result, StatusCodes.Status201Created);
    }
}
