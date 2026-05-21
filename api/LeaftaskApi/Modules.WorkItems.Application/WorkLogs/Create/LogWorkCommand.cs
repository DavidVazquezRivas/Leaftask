using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;

namespace Modules.WorkItems.Application.WorkLogs.Create;

[RequireProjectPermission("Access Project")]
public sealed record LogWorkCommand(
    Guid ProjectId,
    Guid WorkItemId,
    decimal Dedication,
    DateOnly Date,
    string Description) : ICommand<Result<WorkLogDto>>, IProjectPermissionRequest;
