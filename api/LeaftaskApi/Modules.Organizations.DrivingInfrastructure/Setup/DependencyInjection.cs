using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Organizations.Application.Events;
using Modules.Organizations.Application.Management;
using Modules.Organizations.Application.Management.Create;
using Modules.Organizations.Application.Management.GetMyOrganizations;
using Modules.Organizations.Application.Roles.Create;
using Modules.Organizations.Application.Roles.GetPermissions;
using Modules.Organizations.Application.Roles.GetRoles;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.DrivenInfrastructure.Persistence;
using Modules.Organizations.DrivenInfrastructure.Persistence.Seeding;
using Modules.Organizations.DrivenInfrastructure.Queries;
using Modules.Organizations.DrivenInfrastructure.Repositories;
using Modules.Organizations.DrivingInfrastructure.Jobs;
using Modules.Organizations.DrivingInfrastructure.Subscribers;
using Modules.Users.DrivingInfrastructure.Jobs.Outbox;
using Quartz;

namespace Modules.Organizations.DrivingInfrastructure.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddOrganizationsModule(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment)
    {
        services.AddDatabase(configuration);

        services.AddJobs(configuration);
        services.AddMessaging();
        services.AddValidators();

        services.AddRepositories();
        services.AddQueryServices();
        services.AddSeedingStrategy(isDevelopment);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrganizationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Database"));
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        return services;
    }

    private static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
    {
        OutboxOptions outboxOptions = configuration.GetSection("Modules:Organizations:Outbox").Get<OutboxOptions>() ??
                                      new OutboxOptions();

        services.AddQuartz(q => q.AddOutboxJob<OrganizationsOutboxJob>(outboxOptions));
        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(CreateOrganizationCommand).Assembly);
            config.RegisterServicesFromAssembly(typeof(UserCreatedIntegrationEventHandler).Assembly);

            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddSingleton<OrganizationModuleEventMapper>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(GetMyOrganizationsQueryValidator).Assembly);
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationPermissionRepository, OrganizationPermissionRepository>();
        services.AddScoped<IUserReadModelRepository, UserReadModelRepository>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IGetOrganizationDetailsQueryService, GetOrganizationDetailsQueryService>();
        services.AddScoped<IGetMyOrganizationsQueryService, GetMyOrganizationsQueryService>();
        services.AddScoped<IGetOrganizationPermissionsQueryService, GetOrganizationPermissionsQueryService>();
        services.AddScoped<IGetOrganizationRoleDetailsQueryService, GetOrganizationRoleDetailsQueryService>();
        services.AddScoped<IGetOrganizationRolesQueryService, GetOrganizationRolesQueryService>();

        return services;
    }

    private static IServiceCollection AddSeedingStrategy(this IServiceCollection services, bool isDevelopment)
    {
        if (isDevelopment)
        {
            services.AddScoped<IOrganizationSeederStrategy, DevelopmentOrganizationSeeder>();
        }
        else
        {
            services.AddScoped<IOrganizationSeederStrategy, ProductionOrganizationSeeder>();
        }

        return services;
    }
}
