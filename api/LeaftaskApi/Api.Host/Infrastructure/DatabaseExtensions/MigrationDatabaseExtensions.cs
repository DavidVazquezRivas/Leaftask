using System.Net.Sockets;
using Modules.Chats.DrivingInfrastructure.Setup;
using Modules.Organizations.DrivingInfrastructure.Setup;
using Modules.Projects.DrivingInfrastructure.Setup;
using Modules.Users.DrivingInfrastructure.Setup;
using Modules.WorkItems.DrivingInfrastructure.Setup;

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
                await OrganizationsModuleInitialization.ApplyMigrationsAsync(app.Services);
                await ProjectsModuleInitialization.ApplyMigrationsAsync(app.Services);
                await WorkItemsModuleInitialization.ApplyMigrationsAsync(app.Services);
                await ChatsModuleInitialization.ApplyMigrationsAsync(app.Services);
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

            // PostgresException SqlState 57P03 = "the database system is starting up"
            string? sqlState = current.GetType().GetProperty("SqlState")?.GetValue(current) as string;
            if (sqlState is "57P03")
            {
                return true;
            }

            current = current.InnerException;
        }

        return false;
    }
}
