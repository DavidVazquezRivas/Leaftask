using BuildingBlocks.DrivenInfrastructure.Inbox;
using BuildingBlocks.Infrastructure.Events;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Modules.Chats.Application.Events;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;

namespace Modules.Chats.DrivenInfrastructure.Persistence;

public sealed class ChatsDbContext(
    DbContextOptions<ChatsDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher,
    ChatsModuleEventMapper eventMapper) : AppDbContext(options, domainEventsDispatcher, eventMapper)
{
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatParticipant> ChatParticipants { get; set; }
    public DbSet<UserReadModel> UserReadModels { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema.Chat);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChatsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
