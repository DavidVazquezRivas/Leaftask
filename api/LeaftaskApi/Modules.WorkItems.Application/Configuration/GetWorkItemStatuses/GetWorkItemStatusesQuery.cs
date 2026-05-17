using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.Configuration.GetWorkItemStatuses;

public sealed record GetWorkItemStatusesQuery : IQuery<Result<IReadOnlyList<WorkItemStatusDto>>>;
