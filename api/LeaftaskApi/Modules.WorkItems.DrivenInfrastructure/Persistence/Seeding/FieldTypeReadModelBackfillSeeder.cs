using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Persistence.Seeding;

public static class FieldTypeReadModelBackfillSeeder
{
    public static async Task SeedAsync(WorkItemsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO workitem.field_type_read_models ("Id", name)
            SELECT ft."Id", ft.name
            FROM project.field_types ft
            WHERE NOT EXISTS (
                SELECT 1 FROM workitem.field_type_read_models ftrm WHERE ftrm."Id" = ft."Id"
            )
            """, cancellationToken);
    }
}
