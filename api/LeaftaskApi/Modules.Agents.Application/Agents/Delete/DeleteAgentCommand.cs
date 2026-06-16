using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Agents.Application.Authorization;

namespace Modules.Agents.Application.Agents.Delete;

[RequireProjectPermission("agents.delete")]
public sealed record DeleteAgentCommand(Guid AgentId, Guid ProjectId)
    : ICommand<Result>, IProjectPermissionRequest;
