using Microsoft.EntityFrameworkCore;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Repositories;
using Modules.Chats.DrivenInfrastructure.Persistence;

namespace Modules.Chats.DrivenInfrastructure.Repositories;

public sealed class ChatMessageRepository(ChatsDbContext dbContext) : IChatMessageRepository
{
    public async Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default) =>
        await dbContext.ChatMessages.AddAsync(message, cancellationToken);

    public async Task MarkChatMessagesAsReadAsync(Guid chatId, Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.ChatMessages
            .Where(m => EF.Property<Guid>(m, "chat_id") == chatId
                        && !(m.Sender.ParticipantId == userId && m.Sender.ParticipantType == ParticipantType.User)
                        && m.Status != MessageStatus.Read)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.Status, MessageStatus.Read), cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
