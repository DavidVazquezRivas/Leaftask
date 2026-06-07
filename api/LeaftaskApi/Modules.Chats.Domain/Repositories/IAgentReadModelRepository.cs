using Modules.Chats.Domain.Entities.Participant;

namespace Modules.Chats.Domain.Repositories;

public interface IAgentReadModelRepository
{
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AgentReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(AgentReadModel model, CancellationToken cancellationToken = default);
    Task RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
