using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivingInfrastructure.Jobs;

public sealed class WorkItemsOutboxJob(
    IServiceScopeFactory sf,
    ILogger<OutboxJob<WorkItemsDbContext>> l) : OutboxJob<WorkItemsDbContext>(sf, l);
