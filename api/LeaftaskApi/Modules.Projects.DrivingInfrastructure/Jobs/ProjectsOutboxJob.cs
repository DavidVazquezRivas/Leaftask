using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivingInfrastructure.Jobs;

public sealed class ProjectsOutboxJob(
    IServiceScopeFactory sf,
    ILogger<OutboxJob<ProjectsDbContext>> l) : OutboxJob<ProjectsDbContext>(sf, l);
