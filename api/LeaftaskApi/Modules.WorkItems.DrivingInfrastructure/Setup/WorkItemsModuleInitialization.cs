using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.WorkItems.DrivenInfrastructure.Persistence;
using Modules.WorkItems.DrivenInfrastructure.Persistence.Seeding;

namespace Modules.WorkItems.DrivingInfrastructure.Setup;

public static class WorkItemsModuleInitialization
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        WorkItemsDbContext dbContext = scope.ServiceProvider.GetRequiredService<WorkItemsDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        WorkItemsDbContext dbContext = scope.ServiceProvider.GetRequiredService<WorkItemsDbContext>();

        await WorkItemStatusSeeder.SeedAsync(dbContext);
        await WorkItemTypeSeeder.SeedAsync(dbContext);
        await FieldTypeReadModelBackfillSeeder.SeedAsync(dbContext);
        await ProjectReadModelBackfillSeeder.SeedAsync(dbContext);
        await UserReadModelBackfillSeeder.SeedAsync(dbContext);
        await FieldReadModelBackfillSeeder.SeedAsync(dbContext);
    }
}
