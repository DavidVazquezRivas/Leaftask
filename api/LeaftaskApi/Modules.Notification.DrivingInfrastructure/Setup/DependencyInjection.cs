using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Notification.Application.ApprovalRequests.GetMy;
using Modules.Notification.Application.Events;
using Modules.Notification.Application.Notifications.Create;
using Modules.Notification.Application.Notifications.GetMy;
using Modules.Notification.Domain.Repositories;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Notification.DrivenInfrastructure.Queries;
using Modules.Notification.DrivenInfrastructure.Repositories;
using Modules.Notification.DrivingInfrastructure.Controllers;
using Modules.Notification.DrivingInfrastructure.Jobs;
using Modules.Notification.DrivingInfrastructure.Subscribers.Users;
using Quartz;

namespace Modules.Notification.DrivingInfrastructure.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsModule(
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

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        return services;
    }

    private static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
    {
        OutboxOptions outboxOptions =
            configuration.GetSection("Modules:Notifications:Outbox").Get<OutboxOptions>() ?? new OutboxOptions();

        services.AddQuartz(q => q.AddOutboxJob<NotificationsOutboxJob>(outboxOptions));
        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(CreateNotificationCommand).Assembly);
            config.RegisterServicesFromAssembly(typeof(UserCreatedIntegrationEventHandler).Assembly);
            config.RegisterServicesFromAssembly(typeof(NotificationsController).Assembly);

            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddSingleton<NotificationsModuleEventMapper>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IApprovalRequestRepository, ApprovalRequestRepository>();
        services.AddScoped<IUserReadModelRepository, UserReadModelRepository>();
        services.AddScoped<IOrganizationPermissionReadModelRepository, OrganizationPermissionReadModelRepository>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IGetMyNotificationsQueryService, GetMyNotificationsQueryService>();
        services.AddScoped<IGetMyApprovalsQueryService, GetMyApprovalsQueryService>();

        return services;
    }
}
