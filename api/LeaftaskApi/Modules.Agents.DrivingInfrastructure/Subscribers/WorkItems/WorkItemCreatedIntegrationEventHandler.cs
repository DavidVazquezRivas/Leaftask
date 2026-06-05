using System.Text.Json;
using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Application.Agents.EnqueueForEvent;
using Modules.Agents.Domain;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Modules.WorkItems.Integration;

namespace Modules.Agents.DrivingInfrastructure.Subscribers.WorkItems;

public sealed class WorkItemCreatedIntegrationEventHandler(
    AgentsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<WorkItemCreatedIntegrationEvent, AgentsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        WorkItemCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new EnqueueAgentsForEventTriggerCommand(
                AgentEventTypes.WorkItemCreated,
                JsonSerializer.Serialize(notification)),
            cancellationToken);
    }

    protected override Guid GetFallbackMessageId(WorkItemCreatedIntegrationEvent notification) =>
        notification.WorkItemId;
}
