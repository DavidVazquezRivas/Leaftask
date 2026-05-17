using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Domain.Errors;

public static class WorkItemErrors
{
    public static readonly Error WorkItemNotFound =
        new("WorkItem.NotFound", "The specified work item was not found.", 404);
}
