using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivingInfrastructure.Jobs;

public sealed class OrganizationsOutboxJob(
    IServiceScopeFactory sf,
    ILogger<OutboxJob<OrganizationDbContext>> l) : OutboxJob<OrganizationDbContext>(sf, l);
