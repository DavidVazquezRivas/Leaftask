using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Repositories;

public sealed class UserReadModelRepository(WorkItemsDbContext dbContext) : IUserReadModelRepository
{
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.UserReadModels.AsNoTracking()
            .AnyAsync(u => u.Id == id, cancellationToken);

    public async Task AddAsync(UserReadModel userReadModel, CancellationToken cancellationToken = default) =>
        await dbContext.UserReadModels.AddAsync(userReadModel, cancellationToken);
}
