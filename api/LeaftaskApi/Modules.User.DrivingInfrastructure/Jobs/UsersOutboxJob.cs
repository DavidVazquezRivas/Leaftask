using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Users.DrivenInfrastructure.Persistence;

namespace Modules.Users.DrivingInfrastructure.Jobs;

public sealed class UsersOutboxJob(
    IServiceScopeFactory sf,
    ILogger<OutboxJob<UserDbContext>> l) : OutboxJob<UserDbContext>(sf, l);
