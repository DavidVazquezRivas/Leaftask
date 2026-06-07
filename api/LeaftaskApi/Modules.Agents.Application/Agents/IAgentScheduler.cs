namespace Modules.Agents.Application.Agents;

public interface IAgentScheduler
{
    Task ScheduleTimeTriggerAsync(
        Guid agentId,
        Guid triggerId,
        string cronExpression,
        string timeZone,
        CancellationToken cancellationToken = default);

    Task UnscheduleTimeTriggerAsync(Guid triggerId, CancellationToken cancellationToken = default);
}
