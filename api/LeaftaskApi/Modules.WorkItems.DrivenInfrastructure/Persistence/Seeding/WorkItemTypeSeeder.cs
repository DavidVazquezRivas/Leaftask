using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Domain.Entities.Properties;

namespace Modules.WorkItems.DrivenInfrastructure.Persistence.Seeding;

public static class WorkItemTypeSeeder
{
    private static readonly WorkItemType[] Types =
    [
        new(new Guid("b1000000-0001-0000-0000-000000000001"), "Task"),
        new(new Guid("b1000000-0002-0000-0000-000000000001"), "Bug")
    ];

    public static async Task SeedAsync(WorkItemsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        bool alreadySeeded = await dbContext.WorkItemTypes.AnyAsync(cancellationToken);
        if (alreadySeeded)
        {
            return;
        }

        await dbContext.WorkItemTypes.AddRangeAsync(Types, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
