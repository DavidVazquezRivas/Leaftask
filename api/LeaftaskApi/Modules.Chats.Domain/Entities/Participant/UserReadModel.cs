using BuildingBlocks.Domain.Entities;

namespace Modules.Chats.Domain.Entities.Participant;

public sealed class UserReadModel : Entity, IChatParticipant
{
    private UserReadModel() { }

    public UserReadModel(Guid id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public Guid Id { get; }
}
