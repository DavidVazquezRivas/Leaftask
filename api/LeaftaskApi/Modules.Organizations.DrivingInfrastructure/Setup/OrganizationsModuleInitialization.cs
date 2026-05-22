using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Organizations.DrivenInfrastructure.Persistence;
using Modules.Organizations.DrivenInfrastructure.Persistence.Seeding;

namespace Modules.Organizations.DrivingInfrastructure.Setup;

public static class OrganizationsModuleInitialization
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        OrganizationDbContext dbContext = scope.ServiceProvider.GetRequiredService<OrganizationDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        OrganizationDbContext dbContext = scope.ServiceProvider.GetRequiredService<OrganizationDbContext>();
        IOrganizationSeederStrategy seederStrategy = scope.ServiceProvider.GetRequiredService<IOrganizationSeederStrategy>();

        await seederStrategy.SeedAsync(dbContext);
    }
}
