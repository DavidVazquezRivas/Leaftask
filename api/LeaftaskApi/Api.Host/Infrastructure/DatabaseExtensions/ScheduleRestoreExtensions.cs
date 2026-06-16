using Modules.Agents.DrivingInfrastructure.Setup;

namespace Api.Host.Infrastructure.DatabaseExtensions;

internal static class ScheduleRestoreExtensions
{
    public static async Task RestoreSchedulesAsync(this WebApplication app)
    {
        try
        {
            await AgentsModuleInitialization.RestoreTimeTriggerSchedulesAsync(app.Services);
        }
        catch (Exception ex)
        {
            ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error restoring agent time trigger schedules on startup");
        }
    }
}
