using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Integration;
using Modules.WorkItems.Application.Agents.Delete;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivingInfrastructure.Subscribers.Agents;

public sealed class AgentDeletedIntegrationEventHandler(
    WorkItemsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<AgentDeletedIntegrationEvent, WorkItemsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        AgentDeletedIntegrationEvent notification,
        CancellationToken cancellationToken) =>
        await sender.Send(
            new DeleteUserReadModelOnAgentDeletedCommand(notification.AgentId),
            cancellationToken);

    protected override Guid GetFallbackMessageId(AgentDeletedIntegrationEvent notification) =>
        notification.AgentId;
}
