using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application.Events;
using Modules.Users.Application.Management.GetAll;
using Modules.Users.Application.Session.Jwt;
using Modules.Users.Application.Session.OAuth.Google;
using Modules.Users.Domain.Factories;
using Modules.Users.Domain.Repositories;
using Modules.Users.DrivenInfrastructure.Persistence;
using Modules.Users.DrivenInfrastructure.Persistence.Seeding;
using Modules.Users.DrivenInfrastructure.Queries;
using Modules.Users.DrivenInfrastructure.Repositories;
using Modules.Users.DrivenInfrastructure.Session.Jwt;
using Modules.Users.DrivenInfrastructure.Session.OAuth;
using Modules.Users.DrivingInfrastructure.Jobs;
using Modules.Users.DrivingInfrastructure.Jobs.Outbox;
using Quartz;

namespace Modules.Users.DrivingInfrastructure.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersModule(
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

        services.AddSessionServices(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        return services;
    }

    private static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
    {
        OutboxOptions outboxOptions = configuration.GetSection("Modules:Users:Outbox").Get<OutboxOptions>() ??
                                      new OutboxOptions();

        services.AddQuartz(q => q.AddOutboxJob<UsersOutboxJob>(outboxOptions));
        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(GetAllUsersQuery).Assembly);

            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddSingleton<UserModuleEventMapper>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(GetAllUsersQueryValidator).Assembly);

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IGetAllUsersQueryService, GetAllUsersQueryService>();

        return services;
    }

    private static IServiceCollection AddSeedingStrategy(
        this IServiceCollection services,
        bool isDevelopment)
    {
        if (isDevelopment)
        {
            services.AddScoped<IUserSeederStrategy, DevelopmentUserSeeder>();
        }
        else
        {
            services.AddScoped<IUserSeederStrategy, ProductionUserSeeder>();
        }

        return services;
    }

    private static IServiceCollection AddSessionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IJwtService, JwtService>();

        services.Configure<GoogleAuthOptions>(configuration.GetSection(GoogleAuthOptions.SectionName));
        services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();

        services.AddScoped<IRefreshTokenExpirationPolicy, RefreshTokenExpirationPolicy>();
        services.AddScoped<IRefreshTokenFactory, RefreshTokenFactory>();

        return services;
    }
}
