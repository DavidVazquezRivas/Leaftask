using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Agents.Application.Agents;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Entities.AgentTriggers;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Modules.Agents.DrivenInfrastructure.Persistence.Seeding;

namespace Modules.Agents.DrivingInfrastructure.Setup;

public static class AgentsModuleInitialization
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        AgentsDbContext dbContext = scope.ServiceProvider.GetRequiredService<AgentsDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        AgentsDbContext dbContext = scope.ServiceProvider.GetRequiredService<AgentsDbContext>();
        IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await ModelProviderSeeder.SeedAsync(dbContext, configuration);
        await ModelSeeder.SeedAsync(dbContext);
        await ProjectReadModelBackfillSeeder.SeedAsync(dbContext);
    }

    public static async Task RestoreTimeTriggerSchedulesAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        AgentsDbContext dbContext = scope.ServiceProvider.GetRequiredService<AgentsDbContext>();
        IAgentScheduler scheduler = scope.ServiceProvider.GetRequiredService<IAgentScheduler>();
        ILoggerFactory loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger(nameof(AgentsModuleInitialization));

        List<Agent> agents = await dbContext.Agents
            .Include(a => a.TimeTriggers)
            .Where(a => a.TimeTriggers.Count != 0)
            .AsNoTracking()
            .ToListAsync();

        int restored = 0;
        foreach (Agent agent in agents)
        {
            foreach (AgentTimeTrigger trigger in agent.TimeTriggers)
            {
                try
                {
                    await scheduler.ScheduleTimeTriggerAsync(
                        agent.Id,
                        trigger.Id,
                        trigger.CronExpression,
                        trigger.TimeZone);

                    restored++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        "Failed to restore time trigger {TriggerId} for agent {AgentId}",
                        trigger.Id, agent.Id);
                }
            }
        }

        logger.LogInformation(
            "Restored {Count} agent time trigger schedule(s) after startup",
            restored);
    }
}
