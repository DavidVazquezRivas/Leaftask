using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Persistence.Seeding;

public static class ProjectReadModelBackfillSeeder
{
    public static async Task SeedAsync(WorkItemsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO workitem.project_read_models ("Id", abbreviation)
            SELECT p."Id", p.abbreviation
            FROM project.projects p
            WHERE NOT EXISTS (
                SELECT 1 FROM workitem.project_read_models prm WHERE prm."Id" = p."Id"
            )
            """, cancellationToken);
    }
}
