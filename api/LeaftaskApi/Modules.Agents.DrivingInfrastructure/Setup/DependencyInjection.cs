using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;
using BuildingBlocks.DrivingInfrastructure.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Agents.DrivingInfrastructure.Authentication;
using Modules.Agents.Application.Agents;
using Modules.Agents.Application.Bootstrap;
using Modules.Agents.Application.Events;
using Modules.Agents.Application.Kernel;
using Modules.Agents.Application.Projects.Create;
using Modules.Agents.Application.Services;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.DrivenInfrastructure.Bootstrap;
using Modules.Agents.DrivenInfrastructure.KernelFactory;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Modules.Agents.DrivenInfrastructure.Repositories;
using Modules.Agents.DrivingInfrastructure.Jobs;
using Modules.Agents.DrivingInfrastructure.Scheduling;
using Modules.Agents.DrivingInfrastructure.Subscribers.Chats;
using Modules.Agents.DrivingInfrastructure.Subscribers.Projects;
using Modules.Agents.DrivingInfrastructure.Subscribers.WorkItems;
using Modules.Agents.DrivingInfrastructure.Tools;
using Quartz;

namespace Modules.Agents.DrivingInfrastructure.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddAgentsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddJobs(configuration);
        services.AddMessaging();
        services.AddRepositories();
        services.AddModelProvider();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AgentsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        return services;
    }

    private static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
    {
        OutboxOptions outboxOptions = configuration.GetSection("Modules:Agents:Outbox").Get<OutboxOptions>()
                                      ?? new OutboxOptions();

        OutboxOptions executionOptions = configuration.GetSection("Modules:Agents:Execution").Get<OutboxOptions>()
                                         ?? new OutboxOptions { IntervalInSeconds = 10, BatchSize = 5 };

        services.AddQuartz(q =>
        {
            q.AddOutboxJob<AgentsOutboxJob>(outboxOptions);

            JobKey processorKey = new(nameof(AgentExecutionProcessorJob));
            q.AddJob<AgentExecutionProcessorJob>(opts => opts
                .WithIdentity(processorKey)
                .UsingJobData("BatchSize", executionOptions.BatchSize));
            q.AddTrigger(opts => opts
                .ForJob(processorKey)
                .WithIdentity($"{nameof(AgentExecutionProcessorJob)}-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(executionOptions.IntervalInSeconds)
                    .RepeatForever()));

            q.AddJob<AgentTimeTriggerExecutionJob>(opts => opts
                .WithIdentity(nameof(AgentTimeTriggerExecutionJob))
                .StoreDurably());
        });

        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(AgentOrchestrator).Assembly);
            config.RegisterServicesFromAssembly(typeof(CreateProjectReadModelOnProjectCreatedCommand).Assembly);
            config.RegisterServicesFromAssembly(typeof(ProjectCreatedIntegrationEventHandler).Assembly);
            config.RegisterServicesFromAssembly(typeof(UsersMentionedInCommentIntegrationEventHandler).Assembly);
            config.RegisterServicesFromAssembly(typeof(ChatMessageSentIntegrationEventHandler).Assembly);

            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(Modules.Agents.Application.Authorization.ProjectPermissionBehavior<,>));
        });

        services.AddSingleton<AgentsModuleEventMapper>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<IModelRepository, ModelRepository>();
        services.AddScoped<IProjectReadModelRepository, ProjectReadModelRepository>();
        services.AddScoped<IAgentExecutionRepository, AgentExecutionRepository>();
        services.AddScoped<IAgentExecutionMessageRepository, AgentExecutionMessageRepository>();
        services.AddScoped<IAgentExecutionPendingEventRepository, AgentExecutionPendingEventRepository>();

        return services;
    }

    private static IServiceCollection AddModelProvider(this IServiceCollection services)
    {
        services.AddScoped<IAgentKernelFactory, SemanticKernelProvider>();
        services.AddScoped<IBootstrapAgentService, BootstrapAgentService>();
        services.AddScoped<IAgentScheduler>(sp =>
            new QuartzAgentScheduler(sp.GetRequiredService<ISchedulerFactory>()));
        services.AddScoped<AgentExecutionContext>();
        services.AddScoped<AgentSuspensionContext>();
        services.AddScoped<IAiTool, SuspendWorkflowTool>();
        services.AddScoped<AgentOrchestrator>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, AgentAwareUserContext>();

        return services;
    }
}
