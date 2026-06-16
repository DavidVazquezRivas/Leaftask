using BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Notification.DrivenInfrastructure.Persistence;

namespace Modules.Notification.DrivingInfrastructure.Jobs;

public sealed class NotificationsOutboxJob(
    IServiceScopeFactory sf,
    ILogger<OutboxJob<NotificationsDbContext>> l) : OutboxJob<NotificationsDbContext>(sf, l);
