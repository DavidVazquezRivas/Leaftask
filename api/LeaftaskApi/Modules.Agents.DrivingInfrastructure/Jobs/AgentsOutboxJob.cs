using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Agents.DrivenInfrastructure.Persistence;

namespace Modules.Agents.DrivingInfrastructure.Jobs;

public sealed class AgentsOutboxJob(
    IServiceScopeFactory sf,
    ILogger<OutboxJob<AgentsDbContext>> l) : OutboxJob<AgentsDbContext>(sf, l);
