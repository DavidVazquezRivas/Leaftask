using BuildingBlocks.DrivenInfrastructure.Events;
using BuildingBlocks.Infrastructure.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.DrivingInfrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocks(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();

        return services;
    }
}
