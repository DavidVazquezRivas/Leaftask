using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Agents.Application.Events;
using Modules.Agents.Application.Kernel;
using Modules.Agents.Application.Services;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.DrivenInfrastructure.KernelFactory;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Modules.Agents.DrivenInfrastructure.Repositories;
using Modules.Agents.DrivingInfrastructure.Jobs;
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

        services.AddQuartz(q => q.AddOutboxJob<AgentsOutboxJob>(outboxOptions));
        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(AgentOrchestrator).Assembly);

            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddSingleton<AgentsModuleEventMapper>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<IModelRepository, ModelRepository>();

        return services;
    }

    private static IServiceCollection AddModelProvider(this IServiceCollection services)
    {
        services.AddScoped<IAgentKernelFactory, SemanticKernelProvider>();
        services.AddScoped<AgentOrchestrator>();

        return services;
    }
}
