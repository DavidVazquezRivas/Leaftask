using BuildingBlocks.DrivenInfrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Modules.Projects.Application.Events;

namespace Modules.Projects.DrivenInfrastructure.Persistence;

public sealed class ProjectsDbContextFactory : IDesignTimeDbContextFactory<ProjectsDbContext>
{
    public ProjectsDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("DefaultConnection");

        DbContextOptionsBuilder<ProjectsDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(connectionString);

        return new ProjectsDbContext(
            optionsBuilder.Options,
            new NoOpDomainEventsDispatcher(),
            new ProjectModuleEventMapper());
    }
}
