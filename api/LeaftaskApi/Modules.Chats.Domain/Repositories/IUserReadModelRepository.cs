using Modules.Chats.Domain.Entities.Participant;

namespace Modules.Chats.Domain.Repositories;

public interface IUserReadModelRepository
{
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(UserReadModel userReadModel, CancellationToken cancellationToken = default);
}
