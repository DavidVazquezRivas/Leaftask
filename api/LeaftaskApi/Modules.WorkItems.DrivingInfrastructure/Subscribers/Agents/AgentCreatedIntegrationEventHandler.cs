using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Integration;
using Modules.WorkItems.Application.Agents.Create;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivingInfrastructure.Subscribers.Agents;

public sealed class AgentCreatedIntegrationEventHandler(
    WorkItemsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<AgentCreatedIntegrationEvent, WorkItemsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        AgentCreatedIntegrationEvent notification,
        CancellationToken cancellationToken) =>
        await sender.Send(
            new CreateUserReadModelOnAgentCreatedCommand(notification.AgentId, notification.Name),
            cancellationToken);

    protected override Guid GetFallbackMessageId(AgentCreatedIntegrationEvent notification) =>
        notification.AgentId;
}
