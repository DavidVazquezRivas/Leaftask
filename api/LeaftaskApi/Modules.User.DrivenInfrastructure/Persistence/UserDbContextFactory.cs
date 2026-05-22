using BuildingBlocks.DrivenInfrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Modules.Users.Application.Events;

namespace Modules.Users.DrivenInfrastructure.Persistence;

public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("DefaultConnection");

        DbContextOptionsBuilder<UserDbContext> optionsBuilder = new();

        optionsBuilder.UseNpgsql(connectionString);

        return new UserDbContext(optionsBuilder.Options, new NoOpDomainEventsDispatcher(), new UserModuleEventMapper());
    }
}
