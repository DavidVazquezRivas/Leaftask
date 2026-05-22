using Microsoft.EntityFrameworkCore;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Repositories;

public sealed class UserReadModelRepository(ProjectsDbContext dbContext) : IUserReadModelRepository
{
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.UserReadModels.AsNoTracking().AnyAsync(user => user.Id == id, cancellationToken);

    public async Task<UserReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.UserReadModels.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    public async Task AddAsync(UserReadModel userReadModel, CancellationToken cancellationToken = default) =>
        await dbContext.UserReadModels.AddAsync(userReadModel, cancellationToken);
}
