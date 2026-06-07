using Microsoft.EntityFrameworkCore;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Repositories;

public sealed class AgentReadModelRepository(ProjectsDbContext dbContext) : IAgentReadModelRepository
{
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.AgentReadModels.AsNoTracking().AnyAsync(a => a.Id == id, cancellationToken);

    public async Task<AgentReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.AgentReadModels.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task AddAsync(AgentReadModel model, CancellationToken cancellationToken = default) =>
        await dbContext.AgentReadModels.AddAsync(model, cancellationToken);

    public async Task RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.AgentReadModels
            .Where(a => a.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
