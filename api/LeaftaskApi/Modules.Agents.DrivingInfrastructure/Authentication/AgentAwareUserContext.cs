using System.Security.Claims;
using BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Modules.Agents.Application.Services;

namespace Modules.Agents.DrivingInfrastructure.Authentication;

internal sealed class AgentAwareUserContext(
    IHttpContextAccessor httpContextAccessor,
    AgentExecutionContext agentContext) : IUserContext
{
    public Guid UserId =>
        agentContext.IsActive
            ? agentContext.AgentId
            : ParseFromHttpContext();

    public bool IsAuthenticated =>
        agentContext.IsActive
        || (httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false);

    private Guid ParseFromHttpContext() =>
        Guid.TryParse(
            httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out Guid userId)
            ? userId
            : Guid.Empty;
}
