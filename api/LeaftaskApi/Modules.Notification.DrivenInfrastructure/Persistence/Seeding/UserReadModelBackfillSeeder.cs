using Microsoft.EntityFrameworkCore;

namespace Modules.Notification.DrivenInfrastructure.Persistence.Seeding;

public static class UserReadModelBackfillSeeder
{
    public static async Task SeedAsync(NotificationsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO notification.user_read_models ("Id", first_name, last_name)
            SELECT u."Id", u.first_name, u.last_name
            FROM "user".users u
            WHERE NOT EXISTS (
                SELECT 1 FROM notification.user_read_models urm WHERE urm."Id" = u."Id"
            )
            """, cancellationToken);
    }
}
