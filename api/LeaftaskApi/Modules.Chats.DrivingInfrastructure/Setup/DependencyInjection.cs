using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Chats.Application.Chats.List;
using Modules.Chats.Application.Chats.Messages.List;
using Modules.Chats.Application.Chats.Poll;
using Modules.Chats.Application.Events;
using Modules.Chats.Application.Users.Create;
using Modules.Chats.Domain.Repositories;
using Modules.Chats.DrivenInfrastructure.Persistence;
using Modules.Chats.DrivenInfrastructure.Queries;
using Modules.Chats.DrivenInfrastructure.Repositories;
using Modules.Chats.DrivingInfrastructure.Jobs;
using Modules.Chats.DrivingInfrastructure.Subscribers.Agents;
using Modules.Chats.DrivingInfrastructure.Subscribers.Users;
using Quartz;

namespace Modules.Chats.DrivingInfrastructure.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddChatsModule(
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
        services.AddDbContext<ChatsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        return services;
    }

    private static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
    {
        OutboxOptions outboxOptions = configuration.GetSection("Modules:Chats:Outbox").Get<OutboxOptions>() ??
                                     new OutboxOptions();

        services.AddQuartz(q => q.AddOutboxJob<ChatsOutboxJob>(outboxOptions));

        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(CreateUserReadModelOnUserCreatedCommand).Assembly);
            config.RegisterServicesFromAssembly(typeof(UserCreatedIntegrationEventHandler).Assembly);
            config.RegisterServicesFromAssembly(typeof(AgentCreatedIntegrationEventHandler).Assembly);

            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddSingleton<ChatsModuleEventMapper>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IUserReadModelRepository, UserReadModelRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        services.AddScoped<IAgentReadModelRepository, AgentReadModelRepository>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IListChatsQueryService, ListChatsQueryService>();
        services.AddScoped<IPollMessagesService, PollMessagesService>();
        services.AddScoped<IListChatMessagesQueryService, ListChatMessagesQueryService>();

        return services;
    }
}
