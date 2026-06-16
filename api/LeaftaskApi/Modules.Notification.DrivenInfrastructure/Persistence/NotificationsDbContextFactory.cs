using BuildingBlocks.DrivenInfrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Modules.Notification.Application.Events;

namespace Modules.Notification.DrivenInfrastructure.Persistence;

public sealed class NotificationsDbContextFactory : IDesignTimeDbContextFactory<NotificationsDbContext>
{
    public NotificationsDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("Database")!;

        DbContextOptionsBuilder<NotificationsDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(connectionString);

        return new NotificationsDbContext(
            optionsBuilder.Options,
            new NoOpDomainEventsDispatcher(),
            new NotificationsModuleEventMapper());
    }
}
