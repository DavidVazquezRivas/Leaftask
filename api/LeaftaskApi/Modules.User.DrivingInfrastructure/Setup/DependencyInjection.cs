using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application.Management.GetAll;
using Modules.Users.DrivenInfrastructure.Persistence;
using Modules.Users.DrivenInfrastructure.Persistence.Seeding;
using Modules.Users.DrivenInfrastructure.Queries;

namespace Modules.Users.DrivingInfrastructure.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment)
    {
        services.AddDatabase(configuration);
        services.AddMessaging();
        services.AddValidators();
        services.AddRepositories();
        services.AddQueryServices();
        services.AddSeedingStrategy(isDevelopment);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

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

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(GetAllUsersQueryValidator).Assembly);

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services) =>
        //services.AddScoped<IUserRepository, UserRepository>(); User repository is not implemented yet
        services;

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
}
