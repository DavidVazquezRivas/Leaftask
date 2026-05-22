using BuildingBlocks.Application.Commands;

namespace Modules.WorkItems.Application.Projects.Delete;

public sealed record DeleteProjectReadModelOnProjectDeletedCommand(Guid ProjectId) : ICommand;
