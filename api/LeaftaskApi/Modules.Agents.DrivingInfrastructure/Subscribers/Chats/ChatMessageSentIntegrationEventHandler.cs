using System.Text.Json;
using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Application.Agents.DirectQuery;
using Modules.Agents.Application.Agents.EnqueueForEvent;
using Modules.Agents.Application.Agents.Resume;
using Modules.Agents.Domain;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Modules.Chats.Integration;

namespace Modules.Agents.DrivingInfrastructure.Subscribers.Chats;

public sealed class ChatMessageSentIntegrationEventHandler(
    AgentsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<ChatMessageSentIntegrationEvent, AgentsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        ChatMessageSentIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new EnqueueAgentsForEventTriggerCommand(
                AgentEventTypes.ChatMessageSent,
                JsonSerializer.Serialize(notification)),
            cancellationToken);

        HashSet<Guid> resumedAgentIds = [];

        // Skip resume when an agent is the sender — agents must not re-queue themselves via their own messages
        if (!notification.SenderIsAgent)
        {
            resumedAgentIds = await sender.Send(
                new TryResumeAgentExecutionsCommand(
                    AgentEventTypes.ChatMessageSent,
                    notification.ChatId.ToString(),
                    notification.Content),
                cancellationToken);
        }

        foreach (Guid agentId in notification.AgentRecipientIds)
        {
            if (notification.SenderId == agentId) continue;
            if (resumedAgentIds.Contains(agentId)) continue;

            await sender.Send(
                new HandleDirectAgentQueryCommand(agentId, notification.ChatId, notification.Content),
                cancellationToken);
        }
    }

    protected override Guid GetFallbackMessageId(ChatMessageSentIntegrationEvent notification) =>
        notification.MessageId;
}
