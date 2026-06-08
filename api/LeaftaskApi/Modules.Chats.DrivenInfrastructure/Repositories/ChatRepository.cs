using Microsoft.EntityFrameworkCore;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Repositories;
using Modules.Chats.DrivenInfrastructure.Persistence;

namespace Modules.Chats.DrivenInfrastructure.Repositories;

public sealed class ChatRepository(ChatsDbContext dbContext) : IChatRepository
{
    public async Task<Chat?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Chats
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Chat?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Chats
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<ChatParticipant?> GetParticipantAsync(
        Guid chatId, Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.ChatParticipants
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => EF.Property<Guid>(p, "chat_id") == chatId
                     && p.ParticipantId == userId,
                cancellationToken);

    public async Task<ChatParticipant?> GetParticipantTrackedAsync(
        Guid chatId, Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.ChatParticipants
            .FirstOrDefaultAsync(
                p => EF.Property<Guid>(p, "chat_id") == chatId
                     && p.ParticipantId == userId,
                cancellationToken);

    public async Task<bool> ExistsBetweenParticipantsAsync(
        Guid userId, Guid otherParticipantId, ParticipantType otherType,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Guid> userChatIds = dbContext.ChatParticipants
            .Where(p => p.ParticipantId == userId)
            .Select(p => EF.Property<Guid>(p, "chat_id"));

        return await dbContext.ChatParticipants
            .Where(p => userChatIds.Contains(EF.Property<Guid>(p, "chat_id"))
                        && p.ParticipantId == otherParticipantId
                        && p.ParticipantType == otherType)
            .AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetAgentParticipantIdsAsync(Guid chatId,
        CancellationToken cancellationToken = default) =>
        await dbContext.ChatParticipants
            .AsNoTracking()
            .Where(p => EF.Property<Guid>(p, "chat_id") == chatId
                        && p.ParticipantType == ParticipantType.Agent)
            .Select(p => p.ParticipantId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Chat chat, CancellationToken cancellationToken = default) =>
        await dbContext.Chats.AddAsync(chat, cancellationToken);

    public async Task AddParticipantAsync(ChatParticipant participant, CancellationToken cancellationToken = default) =>
        await dbContext.ChatParticipants.AddAsync(participant, cancellationToken);

    public void Remove(Chat chat) =>
        dbContext.Chats.Remove(chat);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
