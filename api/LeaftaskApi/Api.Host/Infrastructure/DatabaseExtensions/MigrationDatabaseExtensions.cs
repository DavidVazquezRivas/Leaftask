using System.Net.Sockets;
using Modules.Organizations.DrivingInfrastructure.Setup;
using Modules.Users.DrivingInfrastructure.Setup;

namespace Api.Host.Infrastructure.DatabaseExtensions;

internal static class MigrationDatabaseExtensions
{
    public static async Task ApplyAllMigrationsAsync(this WebApplication app)
    {
        const int maxAttempts = 10;
        TimeSpan delay = TimeSpan.FromSeconds(3);
        ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await UsersModuleInitialization.ApplyMigrationsAsync(app.Services);
                await OrganizationModuleInitialization.ApplyMigrationsAsync(app.Services);
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts && IsDatabaseUnavailable(ex))
            {
                logger.LogWarning(ex,
                    "Database not ready when applying migrations. Retrying in {DelaySeconds}s ({Attempt}/{MaxAttempts}).",
                    delay.TotalSeconds,
                    attempt,
                    maxAttempts);

                await Task.Delay(delay);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Critical error applying database migrations.");
                throw new InvalidOperationException("Failed to apply database migrations.", ex);
            }
        }
    }

    private static bool IsDatabaseUnavailable(Exception exception)
    {
        Exception? current = exception;

        while (current is not null)
        {
            if (current is SocketException)
            {
                return true;
            }

            current = current.InnerException;
        }

        return false;
    }
}
