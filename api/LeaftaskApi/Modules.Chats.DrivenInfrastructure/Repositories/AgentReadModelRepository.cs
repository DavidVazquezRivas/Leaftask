using Microsoft.EntityFrameworkCore;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Repositories;
using Modules.Chats.DrivenInfrastructure.Persistence;

namespace Modules.Chats.DrivenInfrastructure.Repositories;

public sealed class AgentReadModelRepository(ChatsDbContext dbContext) : IAgentReadModelRepository
{
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.AgentReadModels.AnyAsync(a => a.Id == id, cancellationToken);

    public async Task<AgentReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.AgentReadModels
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task AddAsync(AgentReadModel model, CancellationToken cancellationToken = default) =>
        await dbContext.AgentReadModels.AddAsync(model, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
