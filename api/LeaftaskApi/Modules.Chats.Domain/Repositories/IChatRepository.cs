using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;

namespace Modules.Chats.Domain.Repositories;

public interface IChatRepository
{
    Task<Chat?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Chat?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChatParticipant?> GetParticipantAsync(Guid chatId, Guid userId, CancellationToken cancellationToken = default);
    Task<ChatParticipant?> GetParticipantTrackedAsync(Guid chatId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsBetweenParticipantsAsync(Guid userId, Guid otherParticipantId, ParticipantType otherType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetAgentParticipantIdsAsync(Guid chatId, CancellationToken cancellationToken = default);
    Task AddAsync(Chat chat, CancellationToken cancellationToken = default);
    Task AddParticipantAsync(ChatParticipant participant, CancellationToken cancellationToken = default);
    void Remove(Chat chat);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
