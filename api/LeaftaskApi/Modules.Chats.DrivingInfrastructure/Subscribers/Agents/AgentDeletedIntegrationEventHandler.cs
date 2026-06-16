using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Integration;
using Modules.Chats.Application.Agents.Delete;
using Modules.Chats.DrivenInfrastructure.Persistence;

namespace Modules.Chats.DrivingInfrastructure.Subscribers.Agents;

public sealed class AgentDeletedIntegrationEventHandler(
    ChatsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<AgentDeletedIntegrationEvent, ChatsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        AgentDeletedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new DeleteAgentReadModelOnAgentDeletedCommand(notification.AgentId),
            cancellationToken);
    }

    protected override Guid GetFallbackMessageId(AgentDeletedIntegrationEvent notification) =>
        notification.AgentId;
}
