using System.ComponentModel;
using BuildingBlocks.DrivingInfrastructure.Tools;
using Microsoft.SemanticKernel;
using Modules.Agents.Application.Services;

namespace Modules.Agents.DrivingInfrastructure.Tools;

public sealed class SuspendWorkflowTool(AgentSuspensionContext suspensionContext) : IAiTool
{
    [KernelFunction("SuspendWorkflow")]
    [Description("Pauses the agent execution to wait for asynchronous events, like a human replying in a specific chat or a project event occurring. Call this IMMEDIATELY after sending messages to users that require their response.")]
    public string Suspend(
        [Description("The event type to wait for (e.g. 'chat.message_sent', 'workitem.status_changed').")] string eventType,
        [Description("Comma-separated IDs to wait for (e.g. chatId or projectId).")] string correlationIds)
    {
        List<string> ids = correlationIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        suspensionContext.RequestSuspension(eventType, ids);
        return "[SIGNAL_SUSPEND]";
    }
}
