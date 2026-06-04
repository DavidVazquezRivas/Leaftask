using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Agents.Application.Authorization;

namespace Modules.Agents.Application.Agents.Create;

[RequireProjectPermission("agents.create")]
public sealed record CreateAgentCommand(
    Guid ProjectId,
    string Name,
    string Instructions,
    Guid? TemplateId) : ICommand<Result<AgentDto>>, IProjectPermissionRequest;
