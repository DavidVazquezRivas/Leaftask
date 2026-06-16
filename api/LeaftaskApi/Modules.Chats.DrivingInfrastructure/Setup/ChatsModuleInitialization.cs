using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Chats.DrivenInfrastructure.Persistence;
using Modules.Chats.DrivenInfrastructure.Persistence.Seeding;

namespace Modules.Chats.DrivingInfrastructure.Setup;

public static class ChatsModuleInitialization
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ChatsDbContext dbContext = scope.ServiceProvider.GetRequiredService<ChatsDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ChatsDbContext dbContext = scope.ServiceProvider.GetRequiredService<ChatsDbContext>();

        await UserReadModelBackfillSeeder.SeedAsync(dbContext);
    }
}
