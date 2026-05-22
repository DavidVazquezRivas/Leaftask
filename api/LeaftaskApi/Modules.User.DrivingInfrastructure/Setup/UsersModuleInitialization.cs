using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.DrivenInfrastructure.Persistence;
using Modules.Users.DrivenInfrastructure.Persistence.Seeding;

namespace Modules.Users.DrivingInfrastructure.Setup;

public static class UsersModuleInitialization
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        UserDbContext dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        UserDbContext dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        IUserSeederStrategy seederStrategy = scope.ServiceProvider.GetRequiredService<IUserSeederStrategy>();

        await seederStrategy.SeedAsync(dbContext);
    }
}
