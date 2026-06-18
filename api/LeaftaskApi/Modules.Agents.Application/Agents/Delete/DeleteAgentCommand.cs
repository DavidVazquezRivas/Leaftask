using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.Agents.Application.Agents.Delete;

[RequireProjectPermission("agents.delete")]
public sealed record DeleteAgentCommand(Guid AgentId, Guid ProjectId)
    : ICommand<Result>, IProjectPermissionRequest;
