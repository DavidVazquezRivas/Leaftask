using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BuildingBlocks.DrivingInfrastructure.Jobs.Listeners;

public sealed class LoggingJobListener(ILogger<LoggingJobListener> logger) : IJobListener
{
    public string Name => nameof(LoggingJobListener);

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        string jobName = context.JobDetail.Key.Name;

        logger.LogInformation(
            "[{Listener}] Executing Job: {JobName} (Group: {JobGroup})",
            Name,
            jobName,
            context.JobDetail.Key.Group);

        context.Put("Stopwatch", Stopwatch.StartNew());

        return Task.CompletedTask;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        logger.LogWarning(
            "[{Listener}] Execution of Job {JobName} was vetoed",
            Name,
            context.JobDetail.Key.Name);

        return Task.CompletedTask;
    }

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException,
        CancellationToken cancellationToken = default)
    {
        string jobName = context.JobDetail.Key.Name;
        long elapsedMilliseconds = 0;

        if (context.Get("Stopwatch") is Stopwatch stopwatch)
        {
            stopwatch.Stop();
            elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        }

        if (jobException is not null)
        {
            logger.LogError(
                jobException,
                "[{Listener}] Job {JobName} FAILED after {ElapsedMilliseconds}ms",
                Name,
                jobName,
                elapsedMilliseconds);
        }
        else
        {
            logger.LogInformation(
                "[{Listener}] Job {JobName} completed successfully in {ElapsedMilliseconds}ms",
                Name,
                jobName,
                elapsedMilliseconds);
        }

        return Task.CompletedTask;
    }
}
