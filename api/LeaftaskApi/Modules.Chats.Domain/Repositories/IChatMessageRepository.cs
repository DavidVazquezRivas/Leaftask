using Modules.Chats.Domain.Entities;

namespace Modules.Chats.Domain.Repositories;

public interface IChatMessageRepository
{
    Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default);
    Task MarkChatMessagesAsReadAsync(Guid chatId, Guid userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
