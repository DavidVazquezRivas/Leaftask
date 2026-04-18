using Modules.Organizations.DrivingInfrastructure.Setup;
using Modules.Users.DrivingInfrastructure.Setup;

namespace Api.Host.Infrastructure.DatabaseExtensions;

internal static class SeedDatabaseExtensions
{
    public static async Task SeedAllDataAsync(this WebApplication app)
    {
        try
        {
            await UsersModuleInitialization.SeedDataAsync(app.Services);
            await OrganizationsModuleInitialization.SeedDataAsync(app.Services);
        }
        catch (Exception ex)
        {
            ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error inserting seed data in the database");
        }
    }
}
