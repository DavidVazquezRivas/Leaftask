using System.Text.Json;
using BuildingBlocks.DrivenInfrastructure.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BuildingBlocks.DrivingInfrastructure.Jobs.Outbox;

public abstract class OutboxJob<TContext>(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxJob<TContext>> logger) : IJob where TContext : DbContext
{
    public async Task Execute(IJobExecutionContext context)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        TContext dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
        IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        int batchSize = context.MergedJobDataMap.GetIntValue("BatchSize");
        if (batchSize == 0)
        {
            batchSize = 20;
        }

        List<OutboxMessage> messages = await dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.OccurredAt)
            .Take(batchSize)
            .ToListAsync();

        if (messages.Count == 0)
        {
            logger.LogDebug("No outbox messages to process in module {Module}", typeof(TContext).Name);
            return;
        }

        foreach (OutboxMessage message in messages)
        {
            try
            {
                Type? type = Type.GetType(message.Type);
                if (type is not null)
                {
                    object? integrationEvent = JsonSerializer.Deserialize(message.Content, type);
                    if (integrationEvent is INotification notification)
                    {
                        await publisher.Publish(notification, context.CancellationToken);
                    }
                }

                message.Process();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing the outbox message {Id}", message.Id);
                message.Error = ex.Message;
            }
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
