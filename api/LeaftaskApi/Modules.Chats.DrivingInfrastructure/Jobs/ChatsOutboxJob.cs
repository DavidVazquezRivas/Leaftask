using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Chats.DrivenInfrastructure.Persistence;

namespace Modules.Chats.DrivingInfrastructure.Jobs;

public sealed class ChatsOutboxJob(
    IServiceScopeFactory sf,
    ILogger<OutboxJob<ChatsDbContext>> l) : OutboxJob<ChatsDbContext>(sf, l);
