using System.Text.Json;
using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Application.Agents.EnqueueForEvent;
using Modules.Agents.Application.Agents.Resume;
using Modules.Agents.Domain;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Modules.WorkItems.Integration;

namespace Modules.Agents.DrivingInfrastructure.Subscribers.WorkItems;

public sealed class WorkItemDeletedIntegrationEventHandler(
    AgentsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<WorkItemDeletedIntegrationEvent, AgentsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        WorkItemDeletedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new EnqueueAgentsForEventTriggerCommand(
                AgentEventTypes.WorkItemDeleted,
                JsonSerializer.Serialize(notification)),
            cancellationToken);

        await sender.Send(
            new TryResumeAgentExecutionsCommand(
                AgentEventTypes.WorkItemDeleted,
                notification.ProjectId.ToString(),
                JsonSerializer.Serialize(notification)),
            cancellationToken);
    }

    protected override Guid GetFallbackMessageId(WorkItemDeletedIntegrationEvent notification) =>
        notification.WorkItemId;
}
