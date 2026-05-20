using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.WorkLogs.Delete;

public sealed record DeleteWorkLogCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid LogId) : ICommand<Result>;
