using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Modules.Agents.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/agents")]
public sealed class AgentManagementController : ApiBaseController
{
}
