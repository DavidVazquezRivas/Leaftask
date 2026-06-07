using BuildingBlocks.Domain.Entities;

namespace Modules.Chats.Domain.Entities.Participant;

public sealed class AgentReadModel : Entity, IChatParticipant
{
    private AgentReadModel() { }

    public AgentReadModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}
