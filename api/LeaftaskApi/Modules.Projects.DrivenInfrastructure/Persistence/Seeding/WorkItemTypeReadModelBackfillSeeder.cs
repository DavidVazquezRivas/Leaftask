using Microsoft.EntityFrameworkCore;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Persistence.Seeding;

public static class WorkItemTypeReadModelBackfillSeeder
{
    public static async Task SeedAsync(ProjectsDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO project.workitem_type_read_models ("Id", name)
            SELECT "Id", name
            FROM workitem.work_item_types
            WHERE NOT EXISTS (
                SELECT 1 FROM project.workitem_type_read_models wt WHERE wt."Id" = work_item_types."Id"
            )
            """);
    }
}
