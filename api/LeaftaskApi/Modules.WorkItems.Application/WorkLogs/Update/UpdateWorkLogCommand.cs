using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.WorkLogs.Update;

public sealed record UpdateWorkLogCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid LogId,
    decimal? Dedication,
    DateOnly? Date,
    string? Description) : ICommand<Result<WorkLogDto>>;
