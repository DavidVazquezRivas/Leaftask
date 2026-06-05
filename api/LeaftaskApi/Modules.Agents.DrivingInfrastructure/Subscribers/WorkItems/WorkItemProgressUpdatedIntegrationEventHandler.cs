using System.Text.Json;
using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Application.Agents.EnqueueForEvent;
using Modules.Agents.Domain;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Modules.WorkItems.Integration;

namespace Modules.Agents.DrivingInfrastructure.Subscribers.WorkItems;

public sealed class WorkItemProgressUpdatedIntegrationEventHandler(
    AgentsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<WorkItemProgressUpdatedIntegrationEvent, AgentsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        WorkItemProgressUpdatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new EnqueueAgentsForEventTriggerCommand(
                AgentEventTypes.WorkItemProgressUpdated,
                JsonSerializer.Serialize(notification)),
            cancellationToken);
    }

    protected override Guid GetFallbackMessageId(WorkItemProgressUpdatedIntegrationEvent notification) =>
        notification.WorkItemId;
}
