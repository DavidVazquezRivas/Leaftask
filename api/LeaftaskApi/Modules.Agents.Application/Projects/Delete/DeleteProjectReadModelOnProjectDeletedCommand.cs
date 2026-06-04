using BuildingBlocks.Application.Commands;

namespace Modules.Agents.Application.Projects.Delete;

public sealed record DeleteProjectReadModelOnProjectDeletedCommand(Guid ProjectId) : ICommand;
