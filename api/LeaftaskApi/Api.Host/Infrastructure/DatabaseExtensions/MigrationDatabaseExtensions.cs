using Modules.Organizations.DrivingInfrastructure.Setup;
using Modules.Users.DrivingInfrastructure.Setup;

namespace Api.Host.Infrastructure.DatabaseExtensions;

internal static class MigrationDatabaseExtensions
{
    public static async Task ApplyAllMigrationsAsync(this WebApplication app)
    {
        try
        {
            await UsersModuleInitialization.ApplyMigrationsAsync(app.Services);
            await OrganizationModuleInitialization.ApplyMigrationsAsync(app.Services);
        }
        catch (Exception ex)
        {
            ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogCritical(ex, "Critical error applying database migrations.");
            throw new InvalidOperationException("Failed to apply database migrations.", ex);
        }
    }
}
