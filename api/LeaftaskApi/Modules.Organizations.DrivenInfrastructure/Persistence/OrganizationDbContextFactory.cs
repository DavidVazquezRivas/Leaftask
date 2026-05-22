using BuildingBlocks.DrivenInfrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Modules.Organizations.Application.Events;

namespace Modules.Organizations.DrivenInfrastructure.Persistence;

public class OrganizationDbContextFactory : IDesignTimeDbContextFactory<OrganizationDbContext>
{
    public OrganizationDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("DefaultConnection");

        DbContextOptionsBuilder<OrganizationDbContext> optionsBuilder = new();

        optionsBuilder.UseNpgsql(connectionString);

        return new OrganizationDbContext(optionsBuilder.Options, new NoOpDomainEventsDispatcher(),
            new OrganizationModuleEventMapper());
    }
}
