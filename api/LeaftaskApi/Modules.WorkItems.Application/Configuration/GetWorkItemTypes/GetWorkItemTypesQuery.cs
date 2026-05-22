using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.Configuration.GetWorkItemTypes;

public sealed record GetWorkItemTypesQuery : IQuery<Result<IReadOnlyList<WorkItemTypeDto>>>;
