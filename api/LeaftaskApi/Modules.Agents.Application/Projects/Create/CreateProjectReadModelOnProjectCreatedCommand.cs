using BuildingBlocks.Application.Commands;

namespace Modules.Agents.Application.Projects.Create;

public sealed record CreateProjectReadModelOnProjectCreatedCommand(
    Guid ProjectId,
    string Name) : ICommand;
