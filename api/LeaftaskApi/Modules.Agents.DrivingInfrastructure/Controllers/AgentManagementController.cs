using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Agents.Application.Agents;
using Modules.Agents.Application.Agents.Create;
using Modules.Agents.Application.Agents.Delete;
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

    [HttpDelete("{agentId:guid}")]
    public async Task<IActionResult> DeleteAgent(
        [FromRoute] Guid agentId,
        [FromQuery] Guid projectId,
        CancellationToken cancellationToken = default)
    {
        Result result = await Sender.Send(
            new DeleteAgentCommand(agentId, projectId),
            cancellationToken);

        return HandleResult(result);
    }
}
