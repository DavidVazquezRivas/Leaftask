using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Domain.Errors;

public static class WorkItemErrors
{
    public static readonly Error WorkItemNotFound =
        new("WorkItem.NotFound", "The specified work item was not found.", 404);

    public static readonly Error ProjectNotFound =
        new("WorkItem.Project.NotFound", "The specified project was not found.", 404);

    public static readonly Error AccessDenied =
        new("WorkItem.AccessDenied", "You do not have access to this project.", 403);

    public static readonly Error StatusNotFound =
        new("WorkItem.Status.NotFound", "The specified work item status was not found.", 404);

    public static readonly Error TypeNotFound =
        new("WorkItem.Type.NotFound", "The specified work item type was not found.", 404);

    public static readonly Error AssigneeNotFound =
        new("WorkItem.Assignee.NotFound", "The specified assignee was not found in this project.", 404);

    public static readonly Error ParentNotFound =
        new("WorkItem.Parent.NotFound", "The specified parent work item was not found in this project.", 404);

    public static readonly Error CircularDependency =
        new("WorkItem.CircularDependency", "Setting this parent would create a circular dependency.", 422);

    public static readonly Error RequiredFieldValueMissing =
        new("WorkItem.RequiredFieldValueMissing", "A required custom field has no value.", 422);

    public static readonly Error InvalidFieldValue =
        new("WorkItem.InvalidFieldValue", "The value provided for a custom field is not valid.", 422);

    public static readonly Error WorkLogNotFound =
        new("WorkItem.WorkLog.NotFound", "The specified work log was not found.", 404);

    public static readonly Error WorkLogNotOwner =
        new("WorkItem.WorkLog.NotOwner", "You can only modify your own work logs.", 403);
}
