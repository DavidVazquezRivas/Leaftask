using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Domain.Entities.Properties;

namespace Modules.WorkItems.DrivenInfrastructure.Persistence.Seeding;

public static class WorkItemStatusSeeder
{
    private static readonly WorkItemStatus[] Statuses =
    [
        new(new Guid("a1000000-0001-0000-0000-000000000001"), "Por hacer"),
        new(new Guid("a1000000-0002-0000-0000-000000000001"), "En progreso"),
        new(new Guid("a1000000-0004-0000-0000-000000000001"), "Hecho"),
        new(new Guid("a1000000-0005-0000-0000-000000000001"), "Bloqueado")
    ];

    public static async Task SeedAsync(WorkItemsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        bool alreadySeeded = await dbContext.WorkItemStatuses.AnyAsync(cancellationToken);
        if (alreadySeeded)
        {
            return;
        }

        await dbContext.WorkItemStatuses.AddRangeAsync(Statuses, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
