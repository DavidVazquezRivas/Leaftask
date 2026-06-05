using System.Text.Json;
using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Application.Agents.EnqueueForEvent;
using Modules.Agents.Domain;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Modules.WorkItems.Integration;

namespace Modules.Agents.DrivingInfrastructure.Subscribers.WorkItems;

public sealed class WorkItemStatusChangedIntegrationEventHandler(
    AgentsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<WorkItemStatusChangedIntegrationEvent, AgentsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        WorkItemStatusChangedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new EnqueueAgentsForEventTriggerCommand(
                AgentEventTypes.WorkItemStatusChanged,
                JsonSerializer.Serialize(notification)),
            cancellationToken);
    }

    protected override Guid GetFallbackMessageId(WorkItemStatusChangedIntegrationEvent notification) =>
        notification.WorkItemId;
}
