using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Persistence.Seeding;

public static class UserReadModelBackfillSeeder
{
    public static async Task SeedAsync(WorkItemsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO workitem.user_read_models ("Id", first_name, last_name)
            SELECT u."Id", u.first_name, u.last_name
            FROM "user".users u
            WHERE NOT EXISTS (
                SELECT 1 FROM workitem.user_read_models urm WHERE urm."Id" = u."Id"
            )
            """, cancellationToken);
    }
}
