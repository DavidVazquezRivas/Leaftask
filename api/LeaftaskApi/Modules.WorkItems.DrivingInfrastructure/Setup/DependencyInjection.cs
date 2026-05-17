using BuildingBlocks.Application.Behaviors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.WorkItems.Application.Events;
using Modules.WorkItems.Application.Projects.Create;
using Modules.WorkItems.Application.Configuration.GetWorkItemStatuses;
using Modules.WorkItems.Application.Configuration.GetWorkItemTypes;
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
        services.AddMessaging();
        services.AddRepositories();
        services.AddQueryServices();

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
        });

        services.AddSingleton<WorkItemsModuleEventMapper>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWorkItemRepository, WorkItemRepository>();
        services.AddScoped<IProjectReadModelRepository, ProjectReadModelRepository>();
        services.AddScoped<IUserReadModelRepository, UserReadModelRepository>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IGetWorkItemTypesQueryService, GetWorkItemTypesQueryService>();
        services.AddScoped<IGetWorkItemStatusesQueryService, GetWorkItemStatusesQueryService>();

        return services;
    }
}
