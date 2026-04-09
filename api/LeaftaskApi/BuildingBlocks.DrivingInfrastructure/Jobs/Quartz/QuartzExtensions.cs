using BuildingBlocks.DrivingInfrastructure.Jobs.Listeners;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.DrivingInfrastructure.Jobs.Outbox;
using Quartz;
using Quartz.Impl.Matchers;

namespace BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;

public static class QuartzExtensions
{
    public static IServiceCollection AddQuartzInfrastructure(this IServiceCollection services)
    {
        services.AddQuartz(q => q.AddJobListener<LoggingJobListener>(GroupMatcher<JobKey>.AnyGroup()));

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }

    public static void AddOutboxJob<TJob>(this IServiceCollectionQuartzConfigurator quartz, OutboxOptions options)
        where TJob : IJob
    {
        JobKey jobKey = new(typeof(TJob).Name);

        quartz.AddJob<TJob>(opts => opts
            .WithIdentity(jobKey)
            .UsingJobData("BatchSize", options.BatchSize));

        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity($"{typeof(TJob).Name}-outbox-trigger")
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(options.IntervalInSeconds)
                .RepeatForever()));
    }
}
