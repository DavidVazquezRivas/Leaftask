using Modules.Agents.Application.Agents;
using Modules.Agents.DrivingInfrastructure.Jobs;
using Quartz;

namespace Modules.Agents.DrivingInfrastructure.Scheduling;

public sealed class QuartzAgentScheduler(ISchedulerFactory schedulerFactory) : IAgentScheduler
{
    public async Task ScheduleTimeTriggerAsync(
        Guid agentId,
        Guid triggerId,
        string cronExpression,
        string timeZone,
        CancellationToken cancellationToken = default)
    {
        IScheduler scheduler = await schedulerFactory.GetScheduler(cancellationToken);

        JobKey jobKey = new($"agent-time-{triggerId}", "agent-triggers");

        IJobDetail job = JobBuilder.Create<AgentTimeTriggerExecutionJob>()
            .WithIdentity(jobKey)
            .UsingJobData("agentId", agentId.ToString())
            .UsingJobData("triggerId", triggerId.ToString())
            .StoreDurably()
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"trigger-{triggerId}", "agent-triggers")
            .WithCronSchedule(cronExpression, x =>
                x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(timeZone)))
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}
