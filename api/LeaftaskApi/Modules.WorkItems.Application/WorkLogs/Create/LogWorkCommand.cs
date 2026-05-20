using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.WorkLogs.Create;

public sealed record LogWorkCommand(
    Guid ProjectId,
    Guid WorkItemId,
    decimal Dedication,
    DateOnly Date,
    string Description) : ICommand<Result<WorkLogDto>>;
