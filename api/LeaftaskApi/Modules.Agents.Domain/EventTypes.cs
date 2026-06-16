namespace Modules.Agents.Domain;

public static class AgentEventTypes
{
    public const string ProjectCreated = "project.created";
    public const string ProjectDeleted = "project.deleted";
    public const string WorkItemMention = "workitem.mention";

    public const string WorkItemCreated = "workitem.created";
    public const string WorkItemStatusChanged = "workitem.status_changed";
    public const string WorkItemAssigneeChanged = "workitem.assignee_changed";
    public const string WorkItemDeleted = "workitem.deleted";
    public const string WorkItemProgressUpdated = "workitem.progress_updated";
    public const string WorkItemCommentAdded = "workitem.comment_added";

    public const string ChatMessageSent = "chat.message_sent";
}
