using Microsoft.EntityFrameworkCore;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Repositories;
using Modules.Chats.DrivenInfrastructure.Persistence;

namespace Modules.Chats.DrivenInfrastructure.Repositories;

public sealed class UserReadModelRepository(ChatsDbContext dbContext) : IUserReadModelRepository
{
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.UserReadModels
            .AsNoTracking()
            .AnyAsync(u => u.Id == id, cancellationToken);

    public async Task<UserReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.UserReadModels
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task AddAsync(UserReadModel userReadModel, CancellationToken cancellationToken = default) =>
        await dbContext.UserReadModels.AddAsync(userReadModel, cancellationToken);
}
