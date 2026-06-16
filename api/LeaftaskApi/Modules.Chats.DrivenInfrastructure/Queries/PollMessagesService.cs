using Microsoft.EntityFrameworkCore;
using Modules.Chats.Application.Chats;
using Modules.Chats.Application.Chats.Poll;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.DrivenInfrastructure.Persistence;

namespace Modules.Chats.DrivenInfrastructure.Queries;

public sealed class PollMessagesService(ChatsDbContext dbContext) : IPollMessagesService
{
    public async Task<List<ChatMessageDto>> PollAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        List<ChatParticipant> userParticipants = await dbContext.ChatParticipants
            .Include(p => p.Chat)
            .Where(p => p.ParticipantId == userId && p.ParticipantType == ParticipantType.User)
            .ToListAsync(cancellationToken);

        if (userParticipants.Count == 0)
            return [];

        Dictionary<Guid, DateTime> chatLastFetched = userParticipants
            .ToDictionary(p => p.Chat.Id, p => p.LastFetched);

        List<Guid> chatIds = chatLastFetched.Keys.ToList();
        DateTime globalMin = chatLastFetched.Values.Min();

        List<ChatMessage> allMessages = await dbContext.ChatMessages
            .Include(m => m.Chat)
            .Include(m => m.Sender)
            .Where(m => chatIds.Contains(EF.Property<Guid>(m, "chat_id")) && m.CreatedAt > globalMin)
            .ToListAsync(cancellationToken);

        List<ChatMessage> newMessages = allMessages
            .Where(m => m.CreatedAt > chatLastFetched[m.Chat.Id])
            .ToList();

        DateTime now = DateTime.UtcNow;

        foreach (ChatMessage message in newMessages
            .Where(m => m.Status == MessageStatus.Pending && m.Sender.ParticipantId != userId))
        {
            message.MarkAsDelivered();
        }

        foreach (ChatParticipant participant in userParticipants)
            participant.UpdateLastFetched(now);

        await dbContext.SaveChangesAsync(cancellationToken);

        List<Guid> senderUserIds = newMessages
            .Where(m => m.Sender.ParticipantType == ParticipantType.User)
            .Select(m => m.Sender.ParticipantId)
            .Distinct()
            .ToList();

        Dictionary<Guid, (string FirstName, string LastName)> senderNames = await dbContext.UserReadModels
            .AsNoTracking()
            .Where(u => senderUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => (u.FirstName, u.LastName), cancellationToken);

        return newMessages
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ChatMessageDto(
                m.Id,
                m.Chat.Id,
                m.Content,
                m.CreatedAt,
                MapStatus(m.Status),
                BuildSender(m.Sender, senderNames)))
            .ToList();
    }

    private static string MapStatus(MessageStatus status) => status switch
    {
        MessageStatus.Pending => "pending",
        MessageStatus.Delivered => "delivered",
        MessageStatus.Read => "read",
        _ => status.ToString()
    };

    private static ChatSenderDto BuildSender(
        ChatParticipant sender,
        Dictionary<Guid, (string FirstName, string LastName)> userNames)
    {
        if (sender.ParticipantType == ParticipantType.Agent)
            return new ChatSenderDto(sender.ParticipantId, "AI Assistant", "agent");

        userNames.TryGetValue(sender.ParticipantId, out (string FirstName, string LastName) info);
        string name = $"{info.FirstName} {info.LastName}".Trim();
        return new ChatSenderDto(sender.ParticipantId, name, "person");
    }
}
