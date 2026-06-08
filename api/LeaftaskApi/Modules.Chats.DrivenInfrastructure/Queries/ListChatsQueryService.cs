using Microsoft.EntityFrameworkCore;
using Modules.Chats.Application.Chats;
using Modules.Chats.Application.Chats.List;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.DrivenInfrastructure.Persistence;

namespace Modules.Chats.DrivenInfrastructure.Queries;

public sealed class ListChatsQueryService(ChatsDbContext dbContext) : IListChatsQueryService
{
    public async Task<List<ChatDto>> ListChatsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        List<Guid> chatIds = await dbContext.ChatParticipants
            .AsNoTracking()
            .Where(p => p.ParticipantId == userId)
            .Select(p => EF.Property<Guid>(p, "chat_id"))
            .ToListAsync(cancellationToken);

        if (chatIds.Count == 0)
            return [];

        List<ParticipantRow> participants = await dbContext.ChatParticipants
            .AsNoTracking()
            .Where(p => chatIds.Contains(EF.Property<Guid>(p, "chat_id")))
            .Select(p => new ParticipantRow(
                EF.Property<Guid>(p, "chat_id"),
                p.ParticipantId,
                p.ParticipantType))
            .ToListAsync(cancellationToken);

        List<MessageRow> messages = await dbContext.ChatMessages
            .AsNoTracking()
            .Where(m => chatIds.Contains(EF.Property<Guid>(m, "chat_id")))
            .Select(m => new MessageRow(
                EF.Property<Guid>(m, "chat_id"),
                m.Content,
                m.CreatedAt,
                m.Status))
            .ToListAsync(cancellationToken);

        Dictionary<Guid, int> unreadCountByChat = await dbContext.ChatMessages
            .AsNoTracking()
            .Where(m => chatIds.Contains(EF.Property<Guid>(m, "chat_id"))
                        && m.Sender.ParticipantId != userId
                        && m.Status != MessageStatus.Read)
            .GroupBy(m => EF.Property<Guid>(m, "chat_id"))
            .Select(g => new { ChatId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ChatId, x => x.Count, cancellationToken);

        Dictionary<Guid, MessageRow> lastMessageByChat = messages
            .GroupBy(m => m.ChatId)
            .ToDictionary(g => g.Key, g => g.MaxBy(m => m.Timestamp)!);

        Dictionary<Guid, ParticipantRow> otherParticipantByChat = participants
            .Where(p => p.ParticipantId != userId)
            .GroupBy(p => p.ChatId)
            .ToDictionary(g => g.Key, g => g.First());

        List<Guid> otherParticipantIds = otherParticipantByChat.Values
            .Select(p => p.ParticipantId)
            .Distinct()
            .ToList();

        Dictionary<Guid, string> agentNames = await dbContext.AgentReadModels
            .AsNoTracking()
            .Where(a => otherParticipantIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => a.Name, cancellationToken);

        List<Guid> personOtherIds = otherParticipantIds
            .Where(id => !agentNames.ContainsKey(id))
            .ToList();

        Dictionary<Guid, (string FirstName, string LastName)> userNames = await dbContext.UserReadModels
            .AsNoTracking()
            .Where(u => personOtherIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => (u.FirstName, u.LastName), cancellationToken);

        return chatIds
            .OrderByDescending(id => lastMessageByChat.TryGetValue(id, out MessageRow? msg) ? msg.Timestamp : DateTime.MinValue)
            .Select(id =>
            {
                lastMessageByChat.TryGetValue(id, out MessageRow? lastMsg);
                otherParticipantByChat.TryGetValue(id, out ParticipantRow? other);

                bool isAgent = other is null
                    || other.ParticipantType == ParticipantType.Agent
                    || agentNames.ContainsKey(other.ParticipantId);

                string name;
                string type;

                if (isAgent)
                {
                    agentNames.TryGetValue(other?.ParticipantId ?? Guid.Empty, out string? agentName);
                    name = agentName ?? "AI Assistant";
                    type = "agent";
                }
                else
                {
                    userNames.TryGetValue(other!.ParticipantId, out (string FirstName, string LastName) info);
                    name = $"{info.FirstName} {info.LastName}".Trim();
                    type = "person";
                }

                ChatLastMessageDto? lastMessageDto = lastMsg is null
                    ? null
                    : new ChatLastMessageDto(lastMsg.Content, lastMsg.Timestamp, MapStatus(lastMsg.Status));

                Guid? otherParticipantId = other?.ParticipantId;

                int unreadCount = unreadCountByChat.TryGetValue(id, out int count) ? count : 0;

                return new ChatDto(id, name, lastMessageDto, type, null, otherParticipantId, unreadCount);
            })
            .ToList();
    }

    private static string MapStatus(MessageStatus status) => status switch
    {
        MessageStatus.Pending => "pending",
        MessageStatus.Delivered => "delivered",
        MessageStatus.Read => "read",
        _ => status.ToString()
    };

    private sealed record ParticipantRow(Guid ChatId, Guid ParticipantId, ParticipantType ParticipantType);

    private sealed record MessageRow(Guid ChatId, string Content, DateTime Timestamp, MessageStatus Status);
}
