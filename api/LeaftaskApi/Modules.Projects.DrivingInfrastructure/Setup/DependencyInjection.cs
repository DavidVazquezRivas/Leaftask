using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;
using BuildingBlocks.DrivingInfrastructure.Tools;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Events;
using Modules.Projects.Application.Fields.GetFieldTypes;
using Modules.Projects.Application.Fields.GetProjectCustomFields;
using Modules.Projects.Application.Invitations.GetPending;
using Modules.Projects.Application.Management.Create;
using Modules.Projects.Application.Management.GetMyProjects;
using Modules.Projects.Application.Management.GetOrganizationProjects;
using Modules.Projects.Application.Members.GetMembers;
using Modules.Projects.Application.Organizations.Create;
using Modules.Projects.Application.Organizations.Delete;
using Modules.Projects.Application.Permissions.GetPermissions;
using Modules.Projects.Application.Permissions.GetRoles;
using Modules.Projects.Application.Users.Create;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.DrivenInfrastructure.Authorization;
using Modules.Projects.DrivenInfrastructure.Persistence;
using Modules.Projects.DrivenInfrastructure.Queries;
using Modules.Projects.DrivenInfrastructure.Repositories;
using Modules.Projects.DrivingInfrastructure.Jobs;
using Modules.Projects.DrivingInfrastructure.Services;
using Modules.Projects.DrivingInfrastructure.Subscribers.Organizations;
using Modules.Projects.DrivingInfrastructure.Subscribers.Users;
using Modules.Projects.DrivingInfrastructure.Tools;
using Modules.Projects.Integration;
using Quartz;

namespace Modules.Projects.DrivingInfrastructure.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddJobs(configuration);

        services.AddMessaging();
        services.AddValidators();

        services.AddRepositories();
        services.AddQueryServices();

        services.AddAiTools();

        return services;
    }

    private static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
    {
        OutboxOptions outboxOptions = configuration.GetSection("Modules:Projects:Outbox").Get<OutboxOptions>() ??
                                      new OutboxOptions();

        services.AddQuartz(q => q.AddOutboxJob<ProjectsOutboxJob>(outboxOptions));
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
            config.AddOpenBehavior(typeof(ProjectPermissionBehavior<,>));
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
        services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
        services.AddScoped<IProjectInvitationRepository, ProjectInvitationRepository>();
        services.AddScoped<IUserReadModelRepository, UserReadModelRepository>();
        services.AddScoped<IOrganizationReadModelRepository, OrganizationReadModelRepository>();
        services.AddScoped<IOrganizationPermissionChecker, OrganizationPermissionChecker>();
        services.AddScoped<IProjectPermissionChecker, ProjectPermissionChecker>();
        services.AddScoped<IProjectPermissionService, ProjectPermissionService>();
        services.AddScoped<IProjectRoleRepository, ProjectRoleRepository>();
        services.AddScoped<IProjectFieldRepository, ProjectFieldRepository>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IGetMyProjectsQueryService, GetMyProjectsQueryService>();
        services.AddScoped<IGetOrganizationProjectQueryService, GetOrganizationProjectsQueryService>();
        services.AddScoped<IGetProjectPermissionsQueryService, GetProjectPermissionsQueryService>();
        services.AddScoped<IGetProjectRolesQueryService, GetProjectRolesQueryService>();
        services.AddScoped<IGetProjectMembersQueryService, GetProjectMembersQueryService>();
        services.AddScoped<IGetPendingProjectInvitationsQueryService, GetPendingProjectInvitationsQueryService>();
        services.AddScoped<IGetFieldTypesQueryService, GetFieldTypesQueryService>();
        services.AddScoped<IGetProjectCustomFieldsQueryService, GetProjectCustomFieldsQueryService>();

        return services;
    }

    private static IServiceCollection AddAiTools(this IServiceCollection services)
    {
        services.AddTransient<IAiTool, ProjectCustomFieldsAiTool>();
        services.AddTransient<IAiTool, ProjectInvitationsAiTool>();
        services.AddTransient<IAiTool, ProjectManagementAiTool>();
        services.AddTransient<IAiTool, ProjectMembersAiTool>();
        services.AddTransient<IAiTool, ProjectPermissionsAiTool>();

        return services;
    }
}
