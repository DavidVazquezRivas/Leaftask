using Microsoft.EntityFrameworkCore;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Persistence.Seeding;

public static class UserReadModelBackfillSeeder
{
    public static async Task SeedAsync(ProjectsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO project.users_read_models (id, first_name, last_name, email)
            SELECT u.id, u.first_name, u.last_name, u.email
            FROM "user".users u
            WHERE NOT EXISTS (
                SELECT 1 FROM project.users_read_models ur WHERE ur.id = u.id
            )
            """, cancellationToken);
    }
}
