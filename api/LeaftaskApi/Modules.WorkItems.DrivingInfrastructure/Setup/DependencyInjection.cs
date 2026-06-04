using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.DrivenInfrastructure.Storage;
using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;
using BuildingBlocks.DrivingInfrastructure.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Modules.WorkItems.Application.Authorization;
using Modules.WorkItems.Application.Comments.List;
using Modules.WorkItems.Application.Configuration.GetWorkItemStatuses;
using Modules.WorkItems.Application.Configuration.GetWorkItemTypes;
using Modules.WorkItems.Application.Events;
using Modules.WorkItems.Application.Projects.Create;
using Modules.WorkItems.Application.WorkItems.GetProjectWorkItems;
using Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;
using Modules.WorkItems.Application.WorkLogs.List;
using Modules.WorkItems.Domain.Repositories;
using Modules.WorkItems.DrivenInfrastructure.Persistence;
using Modules.WorkItems.DrivenInfrastructure.Queries;
using Modules.WorkItems.DrivenInfrastructure.Repositories;
using Modules.WorkItems.DrivingInfrastructure.Jobs;
using Modules.WorkItems.DrivingInfrastructure.Subscribers.Projects;
using Modules.WorkItems.DrivingInfrastructure.Tools;
using Quartz;

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
        services.AddFileStorage(configuration);
        services.AddAiTools();

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
        services.AddScoped<IWorkLogRepository, WorkLogRepository>();
        services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IGetWorkItemTypesQueryService, GetWorkItemTypesQueryService>();
        services.AddScoped<IGetWorkItemStatusesQueryService, GetWorkItemStatusesQueryService>();
        services.AddScoped<IGetProjectWorkItemsQueryService, GetProjectWorkItemsQueryService>();
        services.AddScoped<IGetWorkItemDetailsQueryService, GetWorkItemDetailsQueryService>();
        services.AddScoped<IGetWorkLogsQueryService, GetWorkLogsQueryService>();
        services.AddScoped<IGetCommentsQueryService, GetCommentsQueryService>();

        return services;
    }

    private static IServiceCollection AddFileStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        MinioOptions minioOptions = configuration.GetSection("Minio").Get<MinioOptions>() ?? new MinioOptions();

        services.Configure<MinioOptions>(configuration.GetSection("Minio"));

        services.AddMinio(cfg => cfg
            .WithEndpoint(minioOptions.Endpoint)
            .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
            .WithSSL(minioOptions.UseSSL));

        services.AddSingleton<IFileStorage, MinioFileStorage>();

        return services;
    }

    private static IServiceCollection AddAiTools(this IServiceCollection services)
    {
        services.AddTransient<IAiTool, WorkItemAttachmentsAiTool>();
        services.AddTransient<IAiTool, WorkItemCommentsAiTool>();
        services.AddTransient<IAiTool, WorkItemConfigurationAiTool>();
        services.AddTransient<IAiTool, WorkItemManagementAiTool>();
        services.AddTransient<IAiTool, WorkItemTimeTrackingAiTool>();

        return services;
    }
}
