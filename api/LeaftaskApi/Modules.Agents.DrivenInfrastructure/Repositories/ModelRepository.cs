using Microsoft.EntityFrameworkCore;
using Modules.Agents.Domain.Entities.Model;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.DrivenInfrastructure.Persistence;

namespace Modules.Agents.DrivenInfrastructure.Repositories;

public sealed class ModelRepository(AgentsDbContext dbContext) : IModelRepository
{
    public async Task<IReadOnlyList<Model>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Models
            .Include(m => m.Provider)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<Model?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Models
            .Include(m => m.Provider)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
}
