using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Repositories;

public sealed class UserReadModelRepository(OrganizationDbContext dbContext) : IUserReadModelRepository
{
    public async Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.UserReadModels
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId, cancellationToken);

    public async Task AddAsync(UserReadModel userReadModel, CancellationToken cancellationToken = default)
    {
        await dbContext.UserReadModels.AddAsync(userReadModel, cancellationToken);
    }
}
