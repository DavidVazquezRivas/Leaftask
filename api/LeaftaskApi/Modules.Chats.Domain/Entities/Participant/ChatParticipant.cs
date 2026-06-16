using BuildingBlocks.Domain.Entities;

namespace Modules.Chats.Domain.Entities.Participant;

public sealed class ChatParticipant : Entity
{
    private ChatParticipant() { }

    public ChatParticipant(Guid id, Guid participantId, ParticipantType participantType, DateTime lastFetched,
        Chat chat)
    {
        Id = id;
        ParticipantId = participantId;
        ParticipantType = participantType;
        LastFetched = lastFetched;
        Chat = chat;
    }

    public Guid Id { get; }
    public Guid ParticipantId { get; }
    public ParticipantType ParticipantType { get; }
    public DateTime LastFetched { get; private set; }
    public Chat Chat { get; } = null!;

    public void UpdateLastFetched(DateTime lastFetched)
    {
        LastFetched = lastFetched;
    }
}
