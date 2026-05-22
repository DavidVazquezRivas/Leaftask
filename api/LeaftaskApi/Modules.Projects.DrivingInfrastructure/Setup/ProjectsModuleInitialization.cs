using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Projects.DrivenInfrastructure.Persistence;
using Modules.Projects.DrivenInfrastructure.Persistence.Seeding;

namespace Modules.Projects.DrivingInfrastructure.Setup;

public static class ProjectsModuleInitialization
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ProjectsDbContext dbContext = scope.ServiceProvider.GetRequiredService<ProjectsDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        ProjectsDbContext dbContext = scope.ServiceProvider.GetRequiredService<ProjectsDbContext>();

        await ProjectPermissionsSeeder.SeedAsync(dbContext);
        await FieldTypeSeeder.SeedAsync(dbContext);
        await UserReadModelBackfillSeeder.SeedAsync(dbContext);
    }

    public static async Task SeedWorkItemTypeReadModelsAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        ProjectsDbContext dbContext = scope.ServiceProvider.GetRequiredService<ProjectsDbContext>();

        await WorkItemTypeReadModelBackfillSeeder.SeedAsync(dbContext);
    }
}
