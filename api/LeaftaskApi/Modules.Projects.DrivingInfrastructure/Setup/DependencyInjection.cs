using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Events;
using Modules.Projects.Application.Management.Create;
using Modules.Projects.Application.Management.GetMyProjects;
using Modules.Projects.Application.Management.GetOrganizationProjects;
using Modules.Projects.Application.Organizations.Create;
using Modules.Projects.Application.Organizations.Delete;
using Modules.Projects.Application.Users.Create;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.DrivenInfrastructure.Authorization;
using Modules.Projects.DrivenInfrastructure.Persistence;
using Modules.Projects.DrivenInfrastructure.Queries;
using Modules.Projects.DrivenInfrastructure.Repositories;
using Modules.Projects.DrivingInfrastructure.Subscribers.Organizations;
using Modules.Projects.DrivingInfrastructure.Subscribers.Users;

namespace Modules.Projects.DrivingInfrastructure.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddMessaging();
        services.AddValidators();
        services.AddRepositories();
        services.AddQueryServices();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProjectsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(CreateProjectCommand).Assembly);
            config.RegisterServicesFromAssembly(typeof(CreateUserReadModelOnUserCreatedCommand).Assembly);
            config.RegisterServicesFromAssembly(
                typeof(CreateOrganizationReadModelOnOrganizationCreatedCommand).Assembly);
            config.RegisterServicesFromAssembly(
                typeof(DeleteOrganizationReadModelOnOrganizationDeletedCommand).Assembly);

            config.RegisterServicesFromAssembly(typeof(UserCreatedIntegrationEventHandler).Assembly);
            config.RegisterServicesFromAssembly(typeof(OrganizationCreatedIntegrationEventHandler).Assembly);
            config.RegisterServicesFromAssembly(typeof(OrganizationDeletedIntegrationEventHandler).Assembly);

            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(OrganizationPermissionBehavior<,>));
        });

        services.AddSingleton<ProjectModuleEventMapper>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CreateProjectCommandValidator).Assembly);
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IUserReadModelRepository, UserReadModelRepository>();
        services.AddScoped<IOrganizationReadModelRepository, OrganizationReadModelRepository>();
        services.AddScoped<IOrganizationPermissionChecker, OrganizationPermissionChecker>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IGetMyProjectsQueryService, GetMyProjectsQueryService>();
        services.AddScoped<IGetOrganizationProjectQueryService, GetOrganizationProjectsQueryService>();

        return services;
    }
}
