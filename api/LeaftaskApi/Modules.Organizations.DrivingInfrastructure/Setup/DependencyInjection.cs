using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Organizations.Application.Events;
using Modules.Organizations.Application.Management;
using Modules.Organizations.Application.Management.Create;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.DrivenInfrastructure.Persistence;
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
        IConfiguration configuration)
    {
        services.AddDatabase(configuration);

        services.AddJobs(configuration);
        services.AddMessaging();
        services.AddValidators();

        services.AddRepositories();
        services.AddQueryServices();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrganizationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

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

    private static IServiceCollection AddValidators(this IServiceCollection services) => services;

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IUserReadModelRepository, UserReadModelRepository>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IGetBasicOrganizationQueryService, GetBasicOrganizationQueryService>();

        return services;
    }
}
