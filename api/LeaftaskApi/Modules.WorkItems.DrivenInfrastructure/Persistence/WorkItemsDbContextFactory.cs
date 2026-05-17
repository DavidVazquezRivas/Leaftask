using BuildingBlocks.DrivenInfrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Modules.WorkItems.Application.Events;

namespace Modules.WorkItems.DrivenInfrastructure.Persistence;

public sealed class WorkItemsDbContextFactory : IDesignTimeDbContextFactory<WorkItemsDbContext>
{
    public WorkItemsDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("Database");

        DbContextOptionsBuilder<WorkItemsDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(connectionString);

        return new WorkItemsDbContext(
            optionsBuilder.Options,
            new NoOpDomainEventsDispatcher(),
            new WorkItemsModuleEventMapper());
    }
}
