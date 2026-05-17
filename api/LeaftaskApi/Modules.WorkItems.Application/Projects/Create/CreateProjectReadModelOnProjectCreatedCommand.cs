using BuildingBlocks.Application.Commands;

namespace Modules.WorkItems.Application.Projects.Create;

public sealed record CreateProjectReadModelOnProjectCreatedCommand(
    Guid ProjectId,
    string Abbreviation) : ICommand;
