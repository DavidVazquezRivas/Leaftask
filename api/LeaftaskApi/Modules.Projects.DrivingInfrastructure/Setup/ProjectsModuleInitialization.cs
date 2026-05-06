using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivingInfrastructure.Setup;

public static class ProjectsModuleInitialization
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ProjectsDbContext dbContext = scope.ServiceProvider.GetRequiredService<ProjectsDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
