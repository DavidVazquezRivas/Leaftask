using System.Text.Json;
using BuildingBlocks.DrivenInfrastructure.Outbox;
using BuildingBlocks.DrivingInfrastructure.Events;
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
        IIntegrationEventContextAccessor integrationEventContextAccessor =
            scope.ServiceProvider.GetRequiredService<IIntegrationEventContextAccessor>();

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
                integrationEventContextAccessor.CurrentMessageId = message.Id;

                Type type = Type.GetType(message.Type) ??
                            throw new InvalidOperationException(
                                $"Unable to resolve integration event type '{message.Type}'.");

                object? integrationEvent = JsonSerializer.Deserialize(message.Content, type);
                if (integrationEvent is not INotification notification)
                {
                    throw new InvalidOperationException(
                        $"Deserialized message does not implement {nameof(INotification)} for type '{message.Type}'.");
                }

                await publisher.Publish(notification, context.CancellationToken);

                message.Process();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing the outbox message {Id}", message.Id);
                message.Error = ex.Message;
            }
            finally
            {
                integrationEventContextAccessor.CurrentMessageId = null;
            }
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
