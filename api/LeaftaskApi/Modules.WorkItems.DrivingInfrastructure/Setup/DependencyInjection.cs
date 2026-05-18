using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;
using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.WorkItems.DrivingInfrastructure.Jobs;
using Quartz;
using Modules.Projects.Integration;
using Modules.WorkItems.Application.Authorization;
using Modules.WorkItems.Application.Events;
using Modules.WorkItems.Application.Projects.Create;
using Modules.WorkItems.Application.Configuration.GetWorkItemStatuses;
using Modules.WorkItems.Application.Configuration.GetWorkItemTypes;
using Modules.WorkItems.Application.WorkItems.GetProjectWorkItems;
using Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;
using Modules.WorkItems.Domain.Repositories;
using Modules.WorkItems.DrivenInfrastructure.Persistence;
using Modules.WorkItems.DrivenInfrastructure.Queries;
using Modules.WorkItems.DrivenInfrastructure.Repositories;
using Modules.WorkItems.DrivingInfrastructure.Subscribers.Projects;
using Modules.WorkItems.DrivingInfrastructure.Subscribers.Users;

namespace Modules.WorkItems.DrivingInfrastructure.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkItemsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddJobs(configuration);
        services.AddMessaging();
        services.AddRepositories();
        services.AddQueryServices();

        return services;
    }

    private static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
    {
        OutboxOptions outboxOptions = configuration.GetSection("Modules:WorkItems:Outbox").Get<OutboxOptions>() ??
                                      new OutboxOptions();

        services.AddQuartz(q => q.AddOutboxJob<WorkItemsOutboxJob>(outboxOptions));
        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<WorkItemsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(CreateProjectReadModelOnProjectCreatedCommand).Assembly);
            config.RegisterServicesFromAssembly(typeof(ProjectCreatedIntegrationEventHandler).Assembly);

            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(ProjectPermissionBehavior<,>));
        });

        services.AddSingleton<WorkItemsModuleEventMapper>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWorkItemRepository, WorkItemRepository>();
        services.AddScoped<IProjectReadModelRepository, ProjectReadModelRepository>();
        services.AddScoped<IUserReadModelRepository, UserReadModelRepository>();
        services.AddScoped<IWorkItemConfigurationRepository, WorkItemConfigurationRepository>();
        services.AddScoped<IFieldRepository, FieldRepository>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IGetWorkItemTypesQueryService, GetWorkItemTypesQueryService>();
        services.AddScoped<IGetWorkItemStatusesQueryService, GetWorkItemStatusesQueryService>();
        services.AddScoped<IGetProjectWorkItemsQueryService, GetProjectWorkItemsQueryService>();
        services.AddScoped<IGetWorkItemDetailsQueryService, GetWorkItemDetailsQueryService>();

        return services;
    }
}
