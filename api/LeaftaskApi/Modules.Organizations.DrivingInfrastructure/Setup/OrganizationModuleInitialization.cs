using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivingInfrastructure.Setup;

public static class OrganizationModuleInitialization
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        OrganizationDbContext dbContext = scope.ServiceProvider.GetRequiredService<OrganizationDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
