using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Integration;
using Modules.Chats.Application.Agents.Create;
using Modules.Chats.DrivenInfrastructure.Persistence;

namespace Modules.Chats.DrivingInfrastructure.Subscribers.Agents;

public sealed class AgentCreatedIntegrationEventHandler(
    ChatsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<AgentCreatedIntegrationEvent, ChatsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        AgentCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new CreateAgentReadModelOnAgentCreatedCommand(notification.AgentId, notification.Name),
            cancellationToken);
    }

    protected override Guid GetFallbackMessageId(AgentCreatedIntegrationEvent notification) =>
        notification.AgentId;
}
