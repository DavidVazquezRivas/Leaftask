using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Modules.Agents.DrivenInfrastructure.Persistence.Seeding;

namespace Modules.Agents.DrivingInfrastructure.Setup;

public static class AgentsModuleInitialization
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        AgentsDbContext dbContext = scope.ServiceProvider.GetRequiredService<AgentsDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        AgentsDbContext dbContext = scope.ServiceProvider.GetRequiredService<AgentsDbContext>();
        IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await ModelProviderSeeder.SeedAsync(dbContext, configuration);
        await ModelSeeder.SeedAsync(dbContext);
    }
}
