using BuildingBlocks.DrivenInfrastructure.Inbox;
using BuildingBlocks.Infrastructure.Events;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Modules.Agents.Application.Events;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Entities.AgentTriggers;
using Modules.Agents.Domain.Entities.Model;
using Modules.Agents.Domain.Entities.Queue;

namespace Modules.Agents.DrivenInfrastructure.Persistence;

public sealed class AgentsDbContext(
    DbContextOptions<AgentsDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher,
    AgentsModuleEventMapper eventMapper) : AppDbContext(options, domainEventsDispatcher, eventMapper)
{
    public DbSet<Agent> Agents { get; set; }
    public DbSet<AgentTemplate> AgentTemplates { get; set; }
    public DbSet<AgentEventTrigger> AgentEventTriggers { get; set; }
    public DbSet<AgentTimeTrigger> AgentTimeTriggers { get; set; }
    public DbSet<AgentExecutionQueue> AgentExecutionQueues { get; set; }
    public DbSet<ModelProvider> ModelProviders { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<ModelConfig> ModelConfigs { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema.Agent);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgentsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
