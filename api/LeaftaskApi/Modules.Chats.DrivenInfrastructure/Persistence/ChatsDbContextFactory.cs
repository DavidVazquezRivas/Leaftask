using BuildingBlocks.DrivenInfrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Modules.Chats.Application.Events;

namespace Modules.Chats.DrivenInfrastructure.Persistence;

public sealed class ChatsDbContextFactory : IDesignTimeDbContextFactory<ChatsDbContext>
{
    public ChatsDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("Database")!;

        DbContextOptionsBuilder<ChatsDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(connectionString);

        return new ChatsDbContext(
            optionsBuilder.Options,
            new NoOpDomainEventsDispatcher(),
            new ChatsModuleEventMapper());
    }
}
