using Microsoft.EntityFrameworkCore;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Seeding;

public static class ProjectReadModelBackfillSeeder
{
    public static async Task SeedAsync(AgentsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO agent.project_read_models (id, name)
            SELECT p."Id", p.name
            FROM project.projects p
            WHERE NOT EXISTS (
                SELECT 1 FROM agent.project_read_models pr WHERE pr.id = p."Id"
            )
            """, cancellationToken);
    }
}
