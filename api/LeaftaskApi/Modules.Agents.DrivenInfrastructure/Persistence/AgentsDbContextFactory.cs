using BuildingBlocks.DrivenInfrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Modules.Agents.Application.Events;

namespace Modules.Agents.DrivenInfrastructure.Persistence;

public sealed class AgentsDbContextFactory : IDesignTimeDbContextFactory<AgentsDbContext>
{
    public AgentsDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("DefaultConnection")!;

        DbContextOptionsBuilder<AgentsDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(connectionString);

        return new AgentsDbContext(
            optionsBuilder.Options,
            new NoOpDomainEventsDispatcher(),
            new AgentsModuleEventMapper());
    }
}
